namespace user_service.domain.models.@base;

public interface IDbModel
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; }
}
