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

public class UserLogic(
    IDbRepository<User> rep,
    IDbRepository<Role> repRole)
{
    private readonly IDbRepository<User> _rep = rep;
    private readonly IDbRepository<Role> _repRole = repRole;

    public async Task<Result<Profile>> GetMyProfile(Guid userId)
    {
        var resUser = await _rep.GetOne(userId);
        if (resUser.IsFailure)
            return Result<Profile>.Failure(resUser.Error!);

        var profile = new Profile
        {
            UserName = resUser.Value.UserName.Value,
            Email = resUser.Value.Email.Value,
            EmailConfirmed = resUser.Value.ConfirmEmail != null
        };
        
        return Result<Profile>.Success(profile);
    }
}
