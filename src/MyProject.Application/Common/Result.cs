#nullable enable

namespace MyProject.Application.Common;

/// <summary>
/// Represents the result of an operation that can succeed or fail.
/// </summary>
/// <typeparam name="T">The type of the value on success.</typeparam>
public class Result<T>
{
    /// <summary>
    /// Gets whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the value when the operation was successful.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Gets the error message when the operation failed.
    /// </summary>
    public string? Error { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    private Result(string error)
    {
        IsSuccess = false;
        Error = error;
    }

    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    public static Result<T> Ok(T value) => new(value);

    /// <summary>
    /// Creates a failed result with the specified error message.
    /// </summary>
    public static Result<T> Fail(string error) => new(error);
}
