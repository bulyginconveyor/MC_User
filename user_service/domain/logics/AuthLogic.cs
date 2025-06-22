using Microsoft.EntityFrameworkCore;
using user_service.application.dto;
using user_service.domain.models;
using user_service.domain.models.valueobjects;
using user_service.infrastructure.http_clients.interfaces;
using user_service.infrastructure.repository.interfaces;
using user_service.services.hashing;
using user_service.services.jwt_authentification;
using user_service.services.result;
using user_service.services.result.errors;

namespace user_service.domain.logics;

public class AuthLogic(
    IDbRepository<User> rep,
    IDbRepository<Role> repRole,
    ICacheRepository<RegisterData> cacheRegisterData,
    IConfirmCodeStorage confirmCodeStorage,
    INotificationService notificationService,
    JwtTokenHandler jwtTokenHandler,
    IRefreshTokenRepository refreshTokenRepository)
{
    public async Task<Result> GetRegisterData(RegisterData registerData)
    {
        if (string.IsNullOrWhiteSpace(registerData.Email))
            return Result.Failure(Errors.UserLogic.EmailEmpty);
        if (string.IsNullOrWhiteSpace(registerData.Password))
            return Result.Failure(Errors.UserLogic.PasswordEmpty);

        var passwordIsValid = Password.PasswordIsValid(registerData.Password);
        if (passwordIsValid.IsFailure)
            return Result.Failure(Errors.UserLogic.PasswordNotValid);

        var resExistsEmail = await EmailExists(registerData.Email);
        if (resExistsEmail.IsFailure)
            return Result.Failure(resExistsEmail.Error!);
        if (resExistsEmail.Value)
            return Result.Failure(Errors.UserLogic.EmailExists);

        var resAdd = await cacheRegisterData.Add(registerData.Email, registerData);
        if (resAdd.IsFailure)
            return Result.Failure(resAdd.Error!);

        var code = await confirmCodeStorage.GenerateAndSaveCode(registerData.Email);
        await notificationService.SendConfirmLink(registerData.Email, code);

        return Result.Success();
    }
    public async Task<Result<AuthorizeDTO>> ConfrimEmailAndRegister(ConfirmEmailData confrimEmailData)
    {
        var resConfirm = await ConfirmEmail(confrimEmailData);
        if (resConfirm.IsFailure)
            return Result<AuthorizeDTO>.Failure(resConfirm.Error!);

        var resGet = await cacheRegisterData.Get(confrimEmailData.Email);
        if (resGet.IsFailure)
            return Result<AuthorizeDTO>.Failure(resConfirm.Error!);

        var data = resGet.Value!;

        var name = Name.RandomName(data.Email);
        var emailAddress = Email.Create(data.Email);
        var password = Password.Create(data.Password);
        var role = await repRole.GetOne(r => r.Name.Value == "user");
        if (role.IsFailure)
            return Result<AuthorizeDTO>.Failure(resConfirm.Error!);

        var user = new User(name, password, emailAddress, role.Value!);

        //TODO: Добавить проверку на то, что пользователя не существует
        
        var resAdd = await rep.Add(user);
        if (resAdd.IsFailure)
            return Result<AuthorizeDTO>.Failure(resConfirm.Error!);

        var resSave = await rep.Save();

        var token = await refreshTokenRepository.CreateNewToken(user.Role.Name.Value, user.Id);
        
        return resSave.IsFailure
            ? Result<AuthorizeDTO>.Failure(resConfirm.Error!)
            : Result<AuthorizeDTO>.Success(new AuthorizeDTO
            {
                AccessToken = new Token
                {
                    Value = token.Value!.AccessToken,
                    ExpiresSeconds = (int)token.Value.ExpiresAccessToken.Subtract(DateTime.UtcNow).TotalSeconds
                },
                RefreshToken = new Token
                {
                    Value = token.Value.Token,
                    ExpiresSeconds = (long)token.Value.Expires.Subtract(DateTime.UtcNow).TotalSeconds
                }
            });
    }
    public async Task<Result<AuthorizeDTO>> Authenticate(AuthData data)
    {
        if (string.IsNullOrWhiteSpace(data.Login) || string.IsNullOrWhiteSpace(data.Password))
            return Result<AuthorizeDTO>.Failure(Errors.UserLogic.NoAuthorize);
        
        var resUser = await rep.GetOne(u => u.Email.Value == data.Login || u.UserName.Value == data.Login);
        if (resUser.IsFailure)
            return Result<AuthorizeDTO>.Failure(Errors.UserLogic.NoAuthorize);

        if(!PasswordHasher.HashIsPassword(resUser.Value!.Password.Value, data.Password))
            return Result<AuthorizeDTO>.Failure(Errors.UserLogic.NoAuthorize);

        var token = await refreshTokenRepository.CreateNewToken(resUser.Value.Role.Name.Value, resUser.Value.Id);
        
        return Result<AuthorizeDTO>.Success(new AuthorizeDTO
        {
            AccessToken = new Token {
                Value = token.Value.AccessToken,
                ExpiresSeconds = (int)token.Value.ExpiresAccessToken.Subtract(DateTime.UtcNow).TotalSeconds
            },
            RefreshToken = new Token
            {
                Value = token.Value.Token,
                ExpiresSeconds = (long)token.Value.Expires.Subtract(DateTime.UtcNow).TotalSeconds
            }
        });
    }

    public async Task<Result<AuthorizeDTO>> UpdateAccessToken(string token)
    {
        var res = await refreshTokenRepository.UpdateAccessToken(token);
        if(res.IsFailure)
            return Result<AuthorizeDTO>.Failure(res.Error!);

        return Result<AuthorizeDTO>.Success(new AuthorizeDTO
        {
            AccessToken = new Token
            {
                Value = res.Value!.AccessToken,
                ExpiresSeconds = (int)res.Value.ExpiresAccessToken.Subtract(DateTime.UtcNow).TotalSeconds
            },
            RefreshToken = new Token
            {
                Value = res.Value!.Token,
                ExpiresSeconds = (long)res.Value.Expires.Subtract(DateTime.UtcNow).TotalSeconds
            }
        });
    }
    
    private async Task<Result> ConfirmEmail(ConfirmEmailData data)
    {
        if (string.IsNullOrWhiteSpace(data.Email) || string.IsNullOrWhiteSpace(data.Code))
            return Result.Failure(Errors.UserLogic.EmailNotConfirmed);
        
        var resConfirm = await confirmCodeStorage.ConfirmEmail(data.Email, data.Code);
        
        return !resConfirm 
            ? Result.Failure(Errors.UserLogic.EmailNotConfirmed) 
            : Result.Success();
    }

    private async Task<Result<bool>> UserNameExists(string userName)
    {
        var resUserExists = await rep.Exists(u => u.UserName.Value == userName);
        
        return resUserExists.IsFailure 
            ? Result<bool>.Failure(resUserExists.Error!) 
            : Result<bool>.Success(resUserExists.Value);
    }
    private async Task<Result<bool>> EmailExists(string email)
    {
        var resUserExists = await rep.Exists(u => u.Email.Value == email);
        
        return resUserExists.IsFailure 
            ? Result<bool>.Failure(resUserExists.Error!) 
            : Result<bool>.Success(resUserExists.Value);
    }
}
