using user_service.services.result.errors.@base;

namespace user_service.services.result;

public static class ResultExtensions
{
    public static T Match<T>(
        this user_service.services.result.Result result,
        Func<T> success,
        Func<Error, T> failure)
        => result.IsSuccess ? success() : failure(result.Error!);
}
