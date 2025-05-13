using user_service.domain.models.@base;
using user_service.domain.models.valueobjects;
using user_service.services.jwt_authentification;

namespace user_service.domain.models;

public class User : Entity, IDbModel
{
    public Name UserName { get; set; }
    public Password Password { get; set; }
    
    public Email Email { get; set; }

    public Role Role { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; }
    public DateTime? ConfirmEmail { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; }
    
    private User(){}

    public User(Name userName, Password password, Email email, Role role)
    {
        Id = Guid.NewGuid();
        UserName = userName;
        Password = password;
        Email = email;
        Role = role;
    }

    public User(Guid id, Name userName, Password password, Email email, Role role)
    {
        Id = id;
        UserName = userName;
        Password = password;
        Email = email;
        Role = role;
    }
}
