
#if NET5_0_OR_GREATER
using System.Text.Json.Serialization;
#endif

namespace RoyalCode.OperationResult;

/// <summary>
/// <para>
///     Model containing information of an exception, used in the messages of the <see cref="IOperationResult"/>.
/// </para>
/// </summary>
public class ResultMessageException
{
    private readonly Exception? originException;

    /// <summary>
    /// Creates new <see cref="ResultMessageException"/> from a <see cref="Exception"/>.
    /// </summary>
    /// <param name="ex">Exception.</param>
    /// <returns>new instance of <see cref="Exception"/>.</returns>
    public static ResultMessageException FromException(Exception ex) => new ResultMessageException(ex);

    /// <summary>
    /// Creates new <see cref="ResultMessageException"/> from a <see cref="Exception"/>.
    /// </summary>
    /// <param name="ex">Exception.</param>
    private ResultMessageException(Exception ex)
    {
        Message = ex.Message;
        FullNameOfExceptionType = ex.GetType().FullName!;
        StackTrace = ex.StackTrace;
        if (ex.InnerException is not null)
            InnerException = new ResultMessageException(ex.InnerException);

        originException = ex;
    }

#if NET5_0_OR_GREATER

    /// <summary>
    /// <para>
    ///     Creates a new instance with the values of the properties.
    ///     The purpose of this constructor is deserialization.
    /// </para>
    /// </summary>
    /// <param name="message">The Exception message.</param>
    /// <param name="stackTrace">Strack trace da exception.</param>
    /// <param name="fullNameOfExceptionType">The FullName of the exception type.</param>
    /// <param name="innerException">The ResultMessageException instance that represent the System.Exception that caused the current exception.</param>
    /// <exception cref="ArgumentNullException">
    /// <para>
    ///     Case <paramref name="message"/> or <paramref name="fullNameOfExceptionType"/> is null.
    /// </para>
    /// </exception>
    [JsonConstructor]
    public ResultMessageException(
        string message,
        string? stackTrace,
        string fullNameOfExceptionType,
        ResultMessageException? innerException)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        StackTrace = stackTrace;
        FullNameOfExceptionType = fullNameOfExceptionType ?? throw new ArgumentNullException(nameof(fullNameOfExceptionType));
        InnerException = innerException;
    }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets a string representation of the immediate frames on the call stack.
    /// </summary>
    public string? StackTrace { get; }

    /// <summary>
    /// The FullName of the exception type.
    /// </summary>
    public string FullNameOfExceptionType { get; }

    /// <summary>
    /// Gets the ResultMessageException instance that represent the System.Exception that caused the current exception.
    /// </summary>
    public ResultMessageException? InnerException { get; }

#else

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets a string representation of the immediate frames on the call stack.
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// The FullName of the exception type.
    /// </summary>
    public string FullNameOfExceptionType { get; set; }

    /// <summary>
    /// Gets the ResultMessageException instance that represent the System.Exception that caused the current exception.
    /// </summary>
    public ResultMessageException? InnerException { get; set; }

#endif

    /// <summary>
    /// <para>
    ///     Gets the source exception, if not a deserialization of the message.
    /// </para>
    /// <para>
    ///     If there is a serialization and then an deserialization, the value will always be null. 
    ///     If the instance of the object of this type is created from an exception, 
    ///     the source exception, it will be returned.
    /// </para>
    /// </summary>
    /// <returns>
    ///     The source exception, if it was created from an exception, 
    ///     or null if the object was created from a deserialization.
    /// </returns>
    public Exception? GetOriginExcepion() => originException;
}
