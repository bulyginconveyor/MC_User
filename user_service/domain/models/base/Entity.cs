namespace user_service.domain.models.@base;

public class Entity : IEntity<Guid>
{
    public Guid Id { get; set; }
    
    public override bool Equals(object? obj)
    {
        try
        {
            if (obj is null)
                return false;

            var other = (Entity)obj;

            return this.Id == other.Id;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    protected bool Equals(Entity other)
    {
        return Id.Equals(other.Id);
    }
}
