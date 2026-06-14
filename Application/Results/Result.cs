namespace Application.Results;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public ErrorType ErrorType { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    private Result(string error, ErrorType errorType)
    {
        IsSuccess = false;
        Error = error;
        ErrorType = errorType;
    }

    public static Result<T> Ok(T value) => new(value);
    public static Result<T> Failure(string error, ErrorType errorType = ErrorType.BadRequest)
        => new(error, errorType);
}

