using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace user_service.services.jwt_authentification;

public class JwtTokenHandler(string keySecret)
{
    public string jwtSecretKey = keySecret;
    private int jwtTokenValidityMins = 12;
    
    public (string, int) GenerateJwtToken(string role, Guid id)
    {
        var tokenExpiryTimeStamp = DateTime.Now.AddMinutes(jwtTokenValidityMins);
        var tokenKey = Encoding.ASCII.GetBytes(jwtSecretKey);
        var claimsIdentity = new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.UserData, id.ToString())
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

        return (token, (int)tokenExpiryTimeStamp.Subtract(DateTime.Now).TotalSeconds);
    }
}
