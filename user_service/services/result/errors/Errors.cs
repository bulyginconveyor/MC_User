using user_service.domain.logics;
using user_service.domain.models.valueobjects;
using user_service.domain.models.valueobjects.errors;
using user_service.infrastructure.repository.postgresql.repositories.errors;

namespace user_service.services.result.errors;

public class Errors
{
    public static NameErrors Name => new NameErrors();
    public static PasswordErrors Password => new PasswordErrors();
    public static EmailErrors Email => new EmailErrors();
    public static RepositoryErrors Repository => new RepositoryErrors();
    public static UserLogicErrors UserLogic => new UserLogicErrors();
}
