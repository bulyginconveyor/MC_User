using user_service.domain.models.@base;
using user_service.domain.models.valueobjects;

namespace user_service.domain.models;

public class Role : Entity, IDbModel
{
    public Name Name { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; }
    
    private Role(){}

    public Role(Name name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }

    public Role(Guid id, Name name)
    {
        Id = id;
        Name = name;
    }
}
