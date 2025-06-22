using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace user_service.services.jwt_authentification;

public class JwtTokenHandler(string keySecret, int jwtAccessTokenValidityMins = 12, int jwtRefreshTokenValidityDays = 7)
{
    private string jwtSecretKey = keySecret;

    public int JwtRefreshTokenValidityDays => jwtRefreshTokenValidityDays;

    public (string, DateTime) GenerateAccessJwtToken(string role, Guid id)
    {
        var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(jwtAccessTokenValidityMins);
        var tokenKey = Encoding.ASCII.GetBytes(jwtSecretKey);
        var claimsIdentity = new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.Role, role),
            new Claim("uid", id.ToString())
        });

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(tokenKey),
            SecurityAlgorithms.HmacSha256Signature);

        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,
            Expires = tokenExpiryTimeStamp,
            SigningCredentials = signingCredentials
        };

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
        var token = jwtSecurityTokenHandler.WriteToken(securityToken);

        return (token, tokenExpiryTimeStamp);
    }
}
