using Microsoft.EntityFrameworkCore;
using user_service.domain.models;
using user_service.infrastructure.repository.interfaces;
using user_service.services.jwt_authentification;
using user_service.services.result;
using user_service.services.result.errors;

namespace user_service.infrastructure.repository.postgresql.repositories;

public class RefreshTokenRepository(DbContext context, JwtTokenHandler jwtHandler) : IRefreshTokenRepository
{
    public async Task<Result<RefreshToken>> CreateNewToken(string role, Guid userId)
    {
        var refreshToken = GenerateRefreshToken(userId);
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

    public async Task<Result<RefreshToken>> UpdateAccessToken(string refreshTokenData)
    {
        var refreshToken = await context
            .Set<RefreshToken>()
            .FirstOrDefaultAsync(t => t.Token == refreshTokenData);
        
        if (refreshToken is null)
            return Result<RefreshToken>.Failure(Errors.UserLogic.TokenInvalid);
        
        var user = await context.Set<User>()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == refreshToken.UserId);
        
        if (user is null)
            return Result<RefreshToken>.Failure(Errors.UserLogic.UserNotFound);

        if (!refreshToken.IsActive)
            return Result<RefreshToken>.Failure(Errors.UserLogic.NoAuthorize);
        
        context.Set<RefreshToken>().Remove(refreshToken);

        var resCreateNewToken = await CreateNewToken(user.Role.Name.Value, refreshToken.UserId);
        if (resCreateNewToken.IsFailure)
            return resCreateNewToken;
        
        var newRefreshToken = resCreateNewToken.Value!;
        
        await context.SaveChangesAsync();
        
        return Result<RefreshToken>.Success(newRefreshToken);
    }

    public Task<Result> RevorkToken(Guid userId)
    {
        throw new NotImplementedException();
    }

    private (string, DateTime) GenerateRefreshToken(Guid userId)
    {
        var token = Guid.NewGuid().ToString();
        var expires = DateTime.UtcNow.AddDays(jwtHandler.JwtRefreshTokenValidityDays);
        
        return (token, expires);
    }
}
