using user_service.services.result.errors.@base;

namespace user_service.infrastructure.repository.postgresql.repositories.errors;

public class RepositoryErrors
{
    public Error TryException
        => new("Repository.TryException", "Try exception");
    public Error NullArgument
        => new("Repository.NullArgument", "Null argument");
    
    public Error NotFoundGetAll
        => new("Repository.NotFoundGetAll", "Repository not found data");
    public Error NotFoundGetOneById
        => new("Repository.NotFoundGetOneById", "Repository not found data by Id");

    public Error NotFoundExists 
        => new("Repository.NotFoundExists", "Repository not found data filter");

    public Error NotFoundCount
        => new("Repository.NotFoundCount", "Repository not found data count");
}
