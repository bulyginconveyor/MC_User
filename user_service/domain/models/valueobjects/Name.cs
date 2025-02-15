using user_service.services.result;
using user_service.services.result.errors;
using static System.String;

namespace user_service.domain.models.valueobjects;

public record Name
{
    public string Value { get; init; }

    private Name(string value) => Value = IsNullOrEmpty(value) ? null : value;
    
    public static Name Create(string value)
    {
        var res = NameIsValid(value);

        if (res.IsFailure)
            throw new ArgumentException(res.Error!.Description);
        
        return new Name(value);
    }

    public static Result NameIsValid(string value) =>
        IsNullOrWhiteSpace(value) ?
            Result.Failure(Errors.Name.EmptyArgument)
            :
            Result.Success();
    
    public virtual bool Equals(Name? other) => Value == other?.Value;
    public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;

    public bool Contains(string subString) => Value?.ToLower().Contains(subString.ToLower()) ?? false;
    
    public static Name RandomName(string email)
    {
        var name = email.Split('@')[0];
        
        Random random = new Random();
        
        name = $"{name}" +
               $"{random.Next(0, 9)}" +
               $"{random.Next(0, 9)}" +
               $"{random.Next(0, 9)}" +
               $"{random.Next(0, 9)}" +
               $"{random.Next(0, 9)}" +
               $"{random.Next(1, 9)}";

        return Name.Create(name);
    }
}
