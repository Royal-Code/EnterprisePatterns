
namespace RoyalCode.OperationResults;

/// <summary>
/// Extension methods for results and messages.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// It ensures that the errors is success, otherwise it fires a exception.
    /// </summary>
    /// <param name="result">The errors.</param>
    /// <returns>The same instance of <paramref name="result"/>.</returns>
    /// <exception cref="Exception">
    ///     Case the errors is not success.
    /// </exception>
    public static OperationResult EnsureSuccess(this OperationResult result)
    {
        if (result.TryGetError(out var errors))
            throw errors.CreateException();

        return result;
    }

    /// <summary>
    /// It ensures that the errors is success, otherwise it fires a exception.
    /// </summary>
    /// <param name="result">The errors.</param>
    /// <returns>The same instance of <paramref name="result"/>.</returns>
    /// <exception cref="Exception">
    ///     Case the errors is not success.
    /// </exception>
    public static OperationResult<TValue> EnsureSuccess<TValue>(this OperationResult<TValue> result)
    {
        if (result.TryGetError(out var errors))
            throw errors.CreateException();

        return result;
    }

    /// <summary>
    /// It ensures that the errors is success, otherwise it fires a exception.
    /// </summary>
    /// <param name="result">The errors.</param>
    /// <returns>The same instance of <paramref name="result"/>.</returns>
    /// <exception cref="Exception">
    ///     Case the errors is not success.
    /// </exception>
    public static ValidableResult EnsureSuccess(this ValidableResult result)
    {
        if (result.TryGetError(out var errors))
            throw errors.CreateException();

        return result;
    }

    /// <summary>
    /// <para>
    ///     Generate an exception from the message. In case the message contains an original exception, 
    ///     then that exception will be returned, else the property <see cref="IResultMessage.Property"/>
    ///     will be checked if is not null, then an <see cref="ArgumentException"/> will be generated, otherwise
    ///     a <see cref="InvalidOperationException"/> will be generated.
    /// </para>
    /// </summary>
    /// <param name="message">A errors message to extract or generate an exception.</param>
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

    /// <summary>
    /// Get the http status code from the errors.
    /// </summary>
    /// <param name="errors">The operation errors.</param>
    /// <returns>
    ///     The http status code, if any message has an http status code, and if so, returns that has the highest value.
    /// </returns>
    public static int GetHttpStatus(this ResultErrors? errors)
    {
        if (errors is null || errors.Count == 0)
            return 200;

        int httpStatus = 0;
        foreach (var status in errors.Where(m => m.Status is not null).Select(m => (int)m.Status!.Value))
        {
            if (httpStatus == 0
                || (status == 400 && httpStatus == 404) // bad request is better than not found
                || (status == 409 && httpStatus < 500)) // conflict is better than other errors, except server errors
            {
                httpStatus = status;
                continue;
            }

            if (httpStatus >= status)
                continue;

            httpStatus = status;
        }

        return httpStatus == 0 ? 400 : httpStatus;
    }
}
