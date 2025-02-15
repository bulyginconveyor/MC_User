using user_service.services.result.errors.@base;

namespace user_service.domain.models.valueobjects.errors;

public class EmailErrors
{
    public Error EmptyArgument =>
        new Error("Email.EmptyArgument", "Invalid email: Email is spaces or empty or null");
    
    public Error NotValid =>
        new Error("Email.NotValid", "Invalid email: Email is not valid");
}
