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
    JwtTokenHandler jwtTokenHandler)
{
    public async Task<Result> GetRegisterData(RegisterData registerData)
    {
        if (string.IsNullOrWhiteSpace(registerData.UserName))
            return Result.Failure(Errors.UserLogic.UserNameEmpty);
        if (string.IsNullOrWhiteSpace(registerData.Email))
            return Result.Failure(Errors.UserLogic.EmailEmpty);
        if (string.IsNullOrWhiteSpace(registerData.Password))
            return Result.Failure(Errors.UserLogic.PasswordEmpty);

        var passwordIsValid = Password.PasswordIsValid(registerData.Password);
        if (passwordIsValid.IsFailure)
            return Result.Failure(Errors.UserLogic.PasswordNotValid);

        var resExistsUserName = await UserNameExists(registerData.UserName);
        if (resExistsUserName.IsFailure)
            return Result.Failure(resExistsUserName.Error!);
        if(resExistsUserName.Value)
            return Result.Failure(Errors.UserLogic.UserNameExists);
        
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
    public async Task<Result> ConfrimEmailAndRegister(ConfirmEmailData confrimEmailData)
    {
        var resConfirm = await ConfirmEmail(confrimEmailData);
        if (resConfirm.IsFailure)
            return resConfirm;

        var resGet = await cacheRegisterData.Get(confrimEmailData.Email);
        if (resGet.IsFailure)
            return Result.Failure(resGet.Error!);

        var data = resGet.Value!;

        var name = string.IsNullOrWhiteSpace(data.UserName) ? Name.RandomName(data.Email) : Name.Create(data.UserName);
        var emailAddress = Email.Create(data.Email);
        var password = Password.Create(data.Password);
        var role = await repRole.GetOne(r => r.Name.Value == "user");
        if (role.IsFailure)
            return Result.Failure(role.Error!);

        var user = new User(name, password, emailAddress, role.Value!);

        //TODO: добавить проверку на то, что пользователя не существует
        
        var resAdd = await rep.Add(user);
        if (resAdd.IsFailure)
            return Result.Failure(resAdd.Error!);

        var resSave = await rep.Save();
        
        return resSave.IsFailure 
            ? Result.Failure(resSave.Error!) 
            : Result.Success();
    }
    public async Task<Result<Token>> Authenticate(AuthData data)
    {
        if (string.IsNullOrWhiteSpace(data.Login) || string.IsNullOrWhiteSpace(data.Password))
            return Result<Token>.Failure(Errors.UserLogic.NoAuthorize);
        
        var resUser = await rep.GetOne(u => u.Email.Value == data.Login || u.UserName.Value == data.Login);
        if (resUser.IsFailure)
            return Result<Token>.Failure(Errors.UserLogic.NoAuthorize);

        if(!PasswordHasher.HashIsPassword(resUser.Value!.Password.Value, data.Password))
            return Result<Token>.Failure(Errors.UserLogic.NoAuthorize);

        var token = jwtTokenHandler.GenerateJwtToken(resUser.Value.Role.Name.Value, resUser.Value.Id);
        
        return Result<Token>.Success(new Token
        {
            Value = token.Item1,
            ExpiresSeconds = token.Item2
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
