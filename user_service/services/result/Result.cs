using user_service.services.result.errors.@base;

namespace user_service.services.result;

public class Result
{
    private Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
            throw new ArgumentException("Invalid error", nameof(error));
        
        IsSuccess = isSuccess;
        Error = error;
    }
    
    public bool IsSuccess { get; init; }
    public bool IsFailure => !IsSuccess;

    public Error? Error { get; init; }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
}

public class Result<T>
{
    private Result(bool isSuccess, Error error, T value)
    {
        if ((isSuccess && error != Error.None && value == null) ||
            (!isSuccess && error == Error.None && value != null))
            throw new ArgumentException("Invalid error", nameof(error));
        
        IsSuccess = isSuccess;
        Error = error;
        Value = value;
    }

    private Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
            throw new ArgumentException("Invalid error", nameof(error));
        
        IsSuccess = isSuccess;
        Error = error;
    }
    
    public bool IsSuccess { get; init; }
    public bool IsFailure => !IsSuccess;

    public Error? Error { get; init; }

    public T? Value { get; init; }
    
    public static Result<T> Success(T value) => new(true, Error.None, value);
    public static Result<T> Failure(Error error) => new(false, error);
}
