namespace RoyalCode.OperationResult.Exceptions;

/// <summary>
/// <para>
///     Exception to be created from the <see cref="ResultMessageException"/>.
/// </para>
/// <para>
///     This exception contains the stacktrace and the type fullname of the original exception.
/// </para>
/// </summary>
public class InvalidOperationInnerException : Exception
{
    /// <summary>
    /// Create a new exception.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="originalStackTrace"></param>
    /// <param name="fullNameOfOriginalExceptionType"></param>
    public InvalidOperationInnerException(
        string message,
        string? originalStackTrace,
        string fullNameOfOriginalExceptionType) : base(message)
    {
        OriginalStackTrace = originalStackTrace;
        FullNameOfOriginalExceptionType = fullNameOfOriginalExceptionType;
    }

    /// <summary>
    /// Create a new exception, including the inner exception.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    /// <param name="originalStackTrace"></param>
    /// <param name="fullNameOfOriginalExceptionType"></param>
    public InvalidOperationInnerException(
        string message,
        string? originalStackTrace,
        string fullNameOfOriginalExceptionType,
        Exception innerException) : base(message, innerException)
    {
        OriginalStackTrace = originalStackTrace;
        FullNameOfOriginalExceptionType = fullNameOfOriginalExceptionType;
    }

    /// <summary>
    /// Gets a string representation of the immediate frames on the call stack.
    /// </summary>
    public virtual string? OriginalStackTrace { get; private set; }

    /// <summary>
    /// The FullName of the exception type.
    /// </summary>
    public virtual string FullNameOfOriginalExceptionType { get; private set; }
}