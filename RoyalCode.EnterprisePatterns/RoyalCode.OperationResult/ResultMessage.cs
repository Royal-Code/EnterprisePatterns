
using System.Text.Json.Serialization;

namespace RoyalCode.OperationResult;

/// <summary>
/// Component for the message of an operation result.
/// </summary>
public class ResultMessage : IResultMessage
{
    /// <summary>
    /// <para>
    ///     Creates a new message of success.
    /// </para>
    /// </summary>
    /// <param name="text">The message text.</param>
    /// <param name="property">The property related, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <returns>
    /// <para>
    ///     New instance of message.
    /// </para>
    /// </returns>
    public static ResultMessage Success(string text, string? property = null, string? code = null)
    {
        return new ResultMessage(ResultMessageType.Success, text, property, code);
    }

    /// <summary>
    /// <para>
    ///     Creates a new message of information.
    /// </para>
    /// </summary>
    /// <param name="text">The message text.</param>
    /// <param name="property">The property related, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <returns>
    /// <para>
    ///     New instance of message.
    /// </para>
    /// </returns>
    public static ResultMessage Info(string text, string? property = null, string? code = null)
    {
        return new ResultMessage(ResultMessageType.Info, text, property, code);
    }

    /// <summary>
    /// <para>
    ///     Creates a new message of warning.
    /// </para>
    /// </summary>
    /// <param name="text">The message text.</param>
    /// <param name="property">The property related, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <param name="ex">The exception, optional.</param>
    /// <returns>
    /// <para>
    ///     New instance of message.
    /// </para>
    /// </returns>
    public static ResultMessage Warning(string text, string? property = null, string? code = null, Exception? ex = null)
    {
        return new ResultMessage(ResultMessageType.Warning, text, property, code, ex);
    }

    /// <summary>
    /// <para>
    ///     Creates a new message of error.
    /// </para>
    /// </summary>
    /// <param name="text">The message text.</param>
    /// <param name="property">The property related, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <param name="ex">The exception, optional.</param>
    /// <returns>
    /// <para>
    ///     New instance of message.
    /// </para>
    /// </returns>
    public static ResultMessage Error(string text, string? property = null, string? code = null, Exception? ex = null)
    {
        return new ResultMessage(ResultMessageType.Error, text, property, code, ex);
    }

    /// <summary>
    /// <para>
    ///     Creates a new message of error.
    /// </para>
    /// </summary>
    /// <param name="ex">The exception that generate the message.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <returns>
    /// <para>
    ///     New instance of message.
    /// </para>
    /// </returns>
    public static ResultMessage Error(Exception ex, string? property = null, string? code = null)
    {
        if (ex is null)
            throw new ArgumentNullException(nameof(ex));

        return new ResultMessage(ResultMessageType.Error, ex.Message, property, code, ex);
    }

    /// <summary>
    /// <para>
    ///     Creates a error message with the code from <see cref="ResultErrorCodes.NotFound"/>.
    /// </para>
    /// </summary>
    /// <param name="text">The message text.</param>
    /// <returns>
    /// <para>
    ///     New instance of message.
    /// </para>
    /// </returns>
    public static ResultMessage NotFound(string text)
    {
        return new ResultMessage(ResultMessageType.Error, text, null, ResultErrorCodes.NotFound);
    }

    /// <summary>
    /// <para>
    ///     Creates a error message with the code from <see cref="ResultErrorCodes.Forbidden"/>.
    /// </para>
    /// </summary>
    /// <param name="text">The message text.</param>
    /// <returns>
    /// <para>
    ///     New instance of message.
    /// </para>
    /// </returns>
    public static ResultMessage Forbidden(string text)
    {
        return new ResultMessage(ResultMessageType.Error, text, null, ResultErrorCodes.Forbidden);
    }

    /// <summary>
    /// <para>
    ///     Creates a error message with the code from <see cref="ResultErrorCodes.InvalidParameters"/>.
    /// </para>
    /// </summary>
    /// <param name="text">The message text.</param>
    /// <param name="property">The property related, optional.</param>
    /// <returns>
    /// <para>
    ///     New instance of message.
    /// </para>
    /// </returns>
    public static ResultMessage InvalidParameters(string text, string? property = null)
    {
        return new ResultMessage(ResultMessageType.Error, text, property, ResultErrorCodes.InvalidParameters);
    }

    /// <summary>
    /// <para>
    ///     Creates a error message with the code from <see cref="ResultErrorCodes.Validation"/>.
    /// </para>
    /// </summary>
    /// <param name="text">The message text.</param>
    /// <param name="property">The property related, optional.</param>
    /// <param name="ex">The exception, optional.</param>
    /// <returns>
    /// <para>
    ///     New instance of message.
    /// </para>
    /// </returns>
    public static ResultMessage ValidationError(string text, string? property = null, Exception? ex = null)
    {
        return new ResultMessage(ResultMessageType.Error, text, property, ResultErrorCodes.Validation, ex);
    }

    /// <summary>
    /// <para>
    ///     Creates a error message with the code from <see cref="ResultErrorCodes.ApplicationError"/>.
    /// </para>
    /// </summary>
    /// <param name="ex">The exception.</param>
    /// <param name="text">The message text, optional, when not informed will be the exception message.</param>
    /// <returns>
    /// <para>
    ///     New instance of message.
    ///</para>
    /// </returns>
    public static ResultMessage ApplicationError(Exception ex, string? text = null)
    {
        if (ex is null)
            throw new ArgumentNullException(nameof(ex));

        return new ResultMessage(ResultMessageType.Error, text ?? ex.Message, null, ResultErrorCodes.ApplicationError, ex);
    }

    /// <summary>
    /// Creates a new result message.
    /// </summary>
    /// <param name="type">The message type.</param>
    /// <param name="text">The text of the message. The object is to display the message to users.</param>
    /// <param name="property">Property that originated the message.</param>
    /// <param name="code">Some kind of code that can identify the type of message or the message itself.</param>
    /// <param name="exception">Exception related to the message.</param>
    /// <exception cref="ArgumentNullException">
    ///     Case <paramref name="text"/> is null.
    /// </exception>
    [JsonConstructor]
    public ResultMessage(
        ResultMessageType type,
        string text,
        string? property = null,
        string? code = null,
        ResultMessageException? exception = null)
    {
        Type = type;
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Property = property;
        Code = code;
        Exception = exception;
    }

    internal ResultMessage(
        ResultMessageType type,
        string text,
        string? property,
        string? code,
        Exception? exception)
    {
        Type = type;
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Property = property;
        Code = code;
        if (exception is not null)
            Exception = ResultMessageException.FromException(exception);
    }


    /// <inheritdoc/>
    public ResultMessageType Type { get; private set; }

    /// <inheritdoc/>
    public string Text { get; private set; }

    /// <inheritdoc/>
    public string? Property { get; private set; }

    /// <inheritdoc/>
    public string? Code { get; private set; }

    /// <inheritdoc/>
    public ResultMessageException? Exception { get; private set; }

    /// <summary>
    /// Returns the text.
    /// </summary>
    public override string ToString() => Text;
}
