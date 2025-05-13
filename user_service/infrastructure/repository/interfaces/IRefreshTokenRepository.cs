using user_service.services.jwt_authentification;
using user_service.services.result;

namespace user_service.infrastructure.repository.interfaces;

public interface IRefreshTokenRepository
{
    public Task<Result<RefreshToken>> CreateNewToken(string role, Guid userId);
    public Task<Result<RefreshToken>> UpdateAccessToken(string role, Guid userId);
    public Task<Result> RevorkToken(Guid userId);
}
