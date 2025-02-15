using user_service.services.result.errors.@base;

namespace user_service.domain.models.valueobjects.errors;

public class PasswordErrors
{
    public Error EmptyArgument =>
        new Error("Password.EmptyArgument", "Invalid password: Pasword is spaces or empty or null");
    public Error NotUseRules =>
        new Error("Password.NotUseRules", "Invalid password: Password is not use rules");
}
