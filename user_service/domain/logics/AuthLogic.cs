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

public class AuthLogi(
    IDbRepository<User> rep,
    IDbRepository<Role> repRole,
    ICacheRepository<RegisterData> cacheRegisterData,
    IConfirmCodeStorage confirmCodeStorage,
    INotificationServiceREST notificationServiceREST,
    JwtTokenHandler jwtTokenHandler)
{
    private readonly IDbRepository<User> _rep = rep;
    private readonly IDbRepository<Role> _repRole = repRole;
    private readonly ICacheRepository<RegisterData> _cacheRegisterData = cacheRegisterData;
    private readonly IConfirmCodeStorage _confirmCodeStorage = confirmCodeStorage;
    private readonly INotificationServiceREST _notificationServiceREST = notificationServiceREST;
    private readonly JwtTokenHandler _jwtTokenHandler = jwtTokenHandler;

    public async Task<Result> GetRegisterData(RegisterData registerData)
    {
        if (string.IsNullOrWhiteSpace(registerData.UserName))
            return Result.Failure(Errors.UserLogic.UserNameEmpty);
        if (string.IsNullOrWhiteSpace(registerData.Email))
            return Result.Failure(Errors.UserLogic.EmailEmpty);
        if (string.IsNullOrWhiteSpace(registerData.Password))
            return Result.Failure(Errors.UserLogic.PasswordEmpty);

        var passwordIsValid = Password.PasswordIsValid(registerData.Password);
        if (passwordIsValid.IsFailure && passwordIsValid.Error! == Errors.Password.NotUseRules)
            return Result.Failure(Errors.UserLogic.PasswordNotValid);

        var resExistsUserName = await UserNameExists(registerData.UserName);
        if (resExistsUserName.IsFailure)
            return Result.Failure(resExistsUserName.Error);

        var resExistsEmail = await EmailExists(registerData.Email);
        if (resExistsEmail.IsFailure)
            return Result.Failure(resExistsEmail.Error);

        var resAdd = await _cacheRegisterData.Add(registerData.Email, registerData);
        if (resAdd.IsFailure)
            return Result.Failure(resAdd.Error);

        var code = await confirmCodeStorage.GenerateAndSaveCode(registerData.Email);
        await _notificationServiceREST.SendConfirmLink(registerData.Email, code);

        return Result.Success();
    }
    public async Task<Result> ConfrimEmailAndRegister(string email, string code)
    {
        var resConfirm = await ConfirmEmail(email, code);
        if (resConfirm.IsFailure)
            return resConfirm;

        var resGet = await _cacheRegisterData.Get(email);
        if (resGet.IsFailure)
            return Result.Failure(resGet.Error!);

        var data = resGet.Value!;

        Name name = data.UserName == null ? Name.RandomName(data.Email) : Name.Create(data.UserName);
        Email emailAddress = Email.Create(data.Email);
        Password password = Password.Create(data.Password);
        var role = await _repRole.GetOne(r => r.Name.Value == "user");
        if (role.IsFailure)
            return Result.Failure(role.Error!);

        User user = new User(name, password, emailAddress, role.Value!);

        var resAdd = await _rep.Add(user);
        if (resAdd.IsFailure)
            return Result.Failure(resAdd.Error!);

        var resSave = await _rep.Save();
        if (resSave.IsFailure)
            return Result.Failure(resSave.Error!);

        return Result.Success();
    }
    public async Task<Result<Token>> Authenticate(string email, string password)
    {
        var resUser = await _rep.GetOne(u => u.Email.Value == email);
        if (resUser.IsFailure)
            return Result<Token>.Failure(Errors.UserLogic.NoAuthorize);

        if(!PasswordHasher.HashIsPassword(resUser.Value.Password.Value, password))
            return Result<Token>.Failure(Errors.UserLogic.NoAuthorize);

        var token = _jwtTokenHandler.GenerateJwtToken(resUser.Value.Role.Name.Value, resUser.Value.Id);
        
        return Result<Token>.Success(new Token
        {
            Value = token.Item1,
            ExpiresSeconds = token.Item2
        });
    }

    public async Task<Result> ConfirmEmail(string email, string code)
    {
        var resConfirm = await _confirmCodeStorage.ConfirmEmail(email, code);
        if (!resConfirm)
            return Result.Failure(Errors.UserLogic.EmailNotConfirmed);

        return Result.Success();
    }
    public async Task<Result> UserNameExists(string userName)
    {
        var resUserExists = await _rep.Exists(u => u.UserName.Value == userName);
        if (resUserExists.IsFailure || resUserExists.Value)
            return Result.Failure(resUserExists.Error);

        return Result.Success();
    }
    public async Task<Result> EmailExists(string email)
    {
        var resUserExists = await _rep.Exists(u => u.Email.Value == email);
        if (resUserExists.IsFailure || resUserExists.Value)
            return Result.Failure(resUserExists.Error);

        return Result.Success();
    }
}
