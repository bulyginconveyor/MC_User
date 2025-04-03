using System.Text.RegularExpressions;
using user_service.services.hashing;
using user_service.services.result;
using user_service.services.result.errors;

namespace user_service.domain.models.valueobjects;
using static System.String;

public record Password
{
    public string Value { get; init; }

    private Password(string value) => Value = IsNullOrEmpty(value) ? null : value;
    private Password(){}
    public static Password Create(string value)
    {
        var res = PasswordIsValid(value);

        if (res.IsFailure)
            throw new ArgumentException(res.Error!.Description);
        
        return new Password(PasswordHasher.GenerateHash(value));
    }

    public static Password CreateByHashPassword(string hash) => new(hash);

    public static Result PasswordIsValid(string value)
    {
        var res = IsNullOrWhiteSpace(value)
            ? Result.Failure(Errors.Password.EmptyArgument)
            : Result.Success();

        if (res.IsFailure)
            return res;

        res = Regex.IsMatch(value, @"^(?=.*?[a-z])(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[@%&\^$@!#*_-]).{8,}$")
            ? Result.Success()
            : Result.Failure(Errors.Password.NotUseRules);

        return res;
    }

    public virtual bool Equals(Name? other) => Value == other?.Value;
    public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
}
