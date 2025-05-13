using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using user_service.services.result;
using user_service.services.result.errors;

namespace user_service.services.jwt_authentification;

public class JwtHelper
{
    public Result<Guid> UserId(HttpContext context)
    {
        try
        {
            string headers = context.Request.Headers.Authorization!;
            string token = headers.Split(' ')[1];

            var helper = new JwtSecurityTokenHandler();
            var jwt = helper.ReadToken(token);

            var claim = (jwt as JwtSecurityToken).Claims.FirstOrDefault(c => c.Type == ClaimValueTypes.String).Value;

            return Result<Guid>.Success(Guid.Parse(claim));
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure(Errors.TryException);
        }
    }
    public Result<string> UserRole(HttpContext context)
    {
        try
        {
            string headers = context.Request.Headers.Authorization!;
            string token = headers.Split(' ')[1];

            var helper = new JwtSecurityTokenHandler();
            var jwt = helper.ReadToken(token);

            var claim = (jwt as JwtSecurityToken).Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;

            return Result<string>.Success(claim);
        }
        catch (Exception ex)
        {
            return Result<string>.Failure(Errors.TryException);
        }
    }

    public Result<bool> UserIsAdmin(HttpContext context)
    {
        try
        {
            string headers = context.Request.Headers.Authorization!;
            string token = headers.Split(' ')[1];

            var helper = new JwtSecurityTokenHandler();
            var jwt = helper.ReadToken(token);

            var claim = (jwt as JwtSecurityToken).Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;

            return Result<bool>.Success(claim == "Admin");
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(Errors.TryException);
        }
        
    }

    public void LogoutToken(string headers)
    {

    }
}
