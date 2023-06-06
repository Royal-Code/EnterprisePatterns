
namespace RoyalCode.OperationResults;

/// <summary>
/// Extension methods for results and messages.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// It ensures that the result is success, otherwise it fires a <see cref="InvalidOperationException"/>.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>The same instance of <paramref name="result"/>.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Case the result is not success.
    /// </exception>
    public static IOperationResult EnsureSuccess(this IOperationResult result)
    {
        if (result.Success)
            return result;

        throw result.Messages.CreateException();
    }

    /// <summary>
    /// It ensures that the result is success, otherwise it fires a <see cref="InvalidOperationException"/>.
    /// </summary>
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <param name="result">The result.</param>
    /// <returns>The same instance of <paramref name="result"/>.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Case the result is not success.
    /// </exception>
    public static IOperationResult<TValue> EnsureSuccess<TValue>(this IOperationResult<TValue> result)
    {
        if (result.Success)
            return result;

        throw result.Messages.CreateException();
    }

    /// <summary>
    /// <para>
    ///     Join the text of all messagem in one string.
    /// </para>
    /// </summary>
    /// <param name="messages">A collection of messages.</param>
    /// <param name="separator">The separator, by default it is a new line.</param>
    /// <returns>A String that contains the text of all the messages.</returns>
    public static string JoinMessages(this IEnumerable<IResultMessage> messages, string separator = "\n")
    {
        return string.Join(separator, messages);
    }

    /// <summary>
    /// <para>
    ///     Generate an exception from the message. In case the message contains an original exception, 
    ///     then that exception will be returned, else the property <see cref="IResultMessage.Property"/>
    ///     will be checked if is not null, then an <see cref="ArgumentException"/> will be generated, otherwise
    ///     a <see cref="InvalidOperationException"/> will be generated.
    /// </para>
    /// </summary>
    /// <param name="message">A result message to extract or generate an exception.</param>
    /// <returns>
    /// <para>
    ///     An instance of an exception for the message.
    /// </para>
    /// </returns>
    public static Exception ToException(this IResultMessage message)
    {
        return message.Exception 
            ?? (message.Property is null
                ? new InvalidOperationException(message.Text)
                : new ArgumentException(message.Text, message.Property));
    }

    #region Has HttpStatus

    /// <summary>
    /// Get the http status code from the result.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <returns>
    ///     The http status code, if any message has an http status code, and if so, returns that has the highest value.
    /// </returns>
    public static int GetHttpStatus(this IOperationResult result)
    {
        if (result.Success)
            return 200;

        int httpStatus = 0;
        foreach (var status in result.Messages.Where(m => m.Status is not null).Select(m => (int)m.Status!.Value))
        {
            if (status == 404 && httpStatus == 0)
            {
                httpStatus = 404;
                continue;
            }

            if (status > httpStatus)
            {
                httpStatus = status;
            }
        }

        return httpStatus == 0 ? 400 : httpStatus;
    }

    #endregion

    #region Has Error Code

    /// <summary>
    /// <para>
    ///     Checks if the result has a message with the specified error code.
    /// </para>
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="code">The error code to check.</param>
    /// <returns><see langword="true"/> if the result has a message with the specified error code, otherwise <see langword="false"/>.</returns>
    public static bool HasErrorCode(this IOperationResult result, string code)
    {
        if (result is null)
            throw new ArgumentNullException(nameof(result));

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentNullException(nameof(code));

        return result.Messages.Any(m => m.Code == code);
    }

    #endregion

    private static Exception CreateException(this IEnumerable<IResultMessage> messages)
    {
        var exceptions = messages
            .Select(m => m.ToException())
            .ToList();

        Exception exception = exceptions.Count == 1
            ? exceptions.First()
            : new AggregateException("Multiple exceptions have occurred, check the internal exceptions to see the details.", exceptions);

        return exception;
    }
}
