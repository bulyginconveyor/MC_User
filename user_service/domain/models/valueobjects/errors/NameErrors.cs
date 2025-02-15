using user_service.services.result.errors.@base;

namespace user_service.domain.models.valueobjects.errors;

public class NameErrors
{
    public Error EmptyArgument =>
        new Error("Name.EmptyArgument", "Invalid name: Name is spaces or empty or null");
}
