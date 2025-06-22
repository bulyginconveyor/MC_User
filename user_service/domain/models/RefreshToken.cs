using System.ComponentModel.DataAnnotations.Schema;
using user_service.domain.models.@base;

namespace user_service.services.jwt_authentification;

public class RefreshToken
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public string AccessToken { get; set; }
    public DateTime ExpiresAccessToken { get; set; }
    public DateTime Expires { get; set; }
    
    [NotMapped]
    public bool IsExpired => DateTime.UtcNow >= Expires;
    [NotMapped]
    public bool IsAccessExpired => DateTime.UtcNow >= ExpiresAccessToken;
    public DateTime Created { get; set; }
    public DateTime? Revoked { get; set; }
    
    [NotMapped]
    public bool IsActive => Revoked == null && !IsExpired;
}
