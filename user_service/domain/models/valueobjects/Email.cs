using System.Text.RegularExpressions;
using user_service.services.result;
using user_service.services.result.errors;

namespace user_service.domain.models.valueobjects;

public record Email
{
    public string Value { get; set; }

    public static Email Create(string value)
    {
        var res = IsValid(value);
        
        return new Email(value);
    }

    private Email(string value) => Value = value;

    public static Result IsValid(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result.Failure(Errors.Email.EmptyArgument);

        return Regex.IsMatch(value, @"^[a-zA-Z0-9_%+-]+@[a-zA-Z0-9-]{2,}+\.[a-zA-Z]{2,}$")
            ? Result.Success()
            : Result.Failure(Errors.Email.NotValid);
    }
}
