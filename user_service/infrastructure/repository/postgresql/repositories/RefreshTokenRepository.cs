using Microsoft.EntityFrameworkCore;
using user_service.infrastructure.repository.interfaces;
using user_service.services.jwt_authentification;
using user_service.services.result;

namespace user_service.infrastructure.repository.postgresql.repositories;

public class RefreshTokenRepository(DbContext context, JwtTokenHandler jwtHandler) : IRefreshTokenRepository
{
    public async Task<Result<RefreshToken>> CreateNewToken(string role, Guid userId)
    {
        var refreshToken = jwtHandler.GenerateRefreshJwtToken(role, userId);
        var accessToken = jwtHandler.GenerateAccessJwtToken(role, userId);

        RefreshToken newToken = new()
        {
            UserId = userId,
            Token = refreshToken.Item1,
            AccessToken = accessToken.Item1,

            Expires = refreshToken.Item2,
            ExpiresAccessToken = accessToken.Item2,

            Created = DateTime.UtcNow,
        };
        
        await context.Set<RefreshToken>().AddAsync(newToken);
        await context.SaveChangesAsync();
        
        return Result<RefreshToken>.Success(newToken);
    }

    public async Task<Result<RefreshToken>> UpdateAccessToken(string role, Guid userId)
    {
        var refreshToken = await context
            .Set<RefreshToken>()
            .Where(t => t.UserId == userId)
            .OrderBy(t => t.Created)
            .FirstOrDefaultAsync();
        
        if (refreshToken is null)
            return await CreateNewToken(role, userId);
        if (!refreshToken.IsActive)
            return await CreateNewToken(role, userId);
        
        var accessToken = jwtHandler.GenerateAccessJwtToken(role, userId);
        
        refreshToken.AccessToken = accessToken.Item1;
        refreshToken.ExpiresAccessToken = accessToken.Item2;
        
        await context.SaveChangesAsync();
        
        return Result<RefreshToken>.Success(refreshToken);
    }

    public Task<Result> RevorkToken(Guid userId)
    {
        throw new NotImplementedException();
    }
}
