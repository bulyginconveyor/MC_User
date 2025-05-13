using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace user_service.services.jwt_authentification;

public class JwtTokenHandler(string keySecret)
{
    public string jwtSecretKey = keySecret;
    private int jwtAccessTokenValidityMins = 12;
    private int jwtRefreshTokenValidityDays = 91;
    
    public (string, DateTime) GenerateAccessJwtToken(string role, Guid id)
    {
        var tokenExpiryTimeStamp = DateTime.Now.AddMinutes(jwtAccessTokenValidityMins);
        var tokenKey = Encoding.ASCII.GetBytes(jwtSecretKey);
        var claimsIdentity = new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
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
    
    public (string, DateTime) GenerateRefreshJwtToken(string role, Guid id)
    {
        var tokenExpiryTimeStamp = DateTime.Now.AddDays(jwtRefreshTokenValidityDays);
        var tokenKey = Encoding.ASCII.GetBytes(jwtSecretKey);
        var claimsIdentity = new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
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
