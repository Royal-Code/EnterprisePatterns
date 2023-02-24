
using System.Net;

namespace RoyalCode.OperationResult;

/// <summary>
/// Extension methods for results and messages.
/// </summary>
public static class ResultMessageExtensions
{
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
    ///     that exception will be returned, otherwise a <see cref="InvalidOperationException"/> will be generated.
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
        return message.Exception ?? message.ToInvalidOperationException();
    }

    /// <summary>
    /// <para>
    ///     Gererate a <see cref="InvalidOperationException"/> from the result message.
    /// </para>
    /// </summary>
    /// <param name="message">The result message.</param>
    /// <returns>
    /// <para>
    ///     One instance of <see cref="InvalidOperationException"/> for the message.
    /// </para>
    /// </returns>
    public static InvalidOperationException ToInvalidOperationException(this IResultMessage message)
    {
        var originalException = message.Exception;

        return originalException is not null
            ? originalException is InvalidOperationException ioex
                ? ioex
                : new InvalidOperationException(message.Text, originalException)
            : new InvalidOperationException(message.Text);
    }

    /// <summary>
    /// <para>
    ///     Set a new value for the message text.
    /// </para>
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="text">The new text.</param>
    /// <returns>The same instance of <see cref="ResultMessage"/>. for chaining.</returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="message"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <paramref name="text"/> is null or empty.
    /// </exception>
    public static ResultMessage WithText(this ResultMessage message, string text)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));

        if (string.IsNullOrEmpty(text))
            throw new ArgumentException($"'{nameof(text)}' cannot be null or empty.", nameof(text));

        message.Text= text;
        return message;
    }

    /// <summary>
    /// <para>
    ///     Set a new value for the message property.
    /// </para>
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="property">The new property.</param>
    /// <returns>The same instance of <see cref="ResultMessage"/>. for chaining.</returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="message"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <paramref name="property"/> is null or empty.
    /// </exception>
    public static ResultMessage WithProperty(this ResultMessage message, string property)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));

        if (string.IsNullOrEmpty(property))
            throw new ArgumentException($"'{nameof(property)}' cannot be null or empty.", nameof(property));

        message.Property = property;
        return message;
    }

    /// <summary>
    /// <para>
    ///     Set a new value for the message code.
    /// </para>
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="code">The new code.</param>
    /// <returns>The same instance of <see cref="ResultMessage"/>. for chaining.</returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="message"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <paramref name="code"/> is null or empty.
    /// </exception>
    public static ResultMessage WithCode(this ResultMessage message, string code)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));
        if (string.IsNullOrEmpty(code))
            throw new ArgumentException($"'{nameof(code)}' cannot be null or empty.", nameof(code));

        message.Code = code;
        return message;
    }

    /// <summary>
    /// <para>
    ///     Set a new value for the message status.
    /// </para>
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="status">The new status.</param>
    /// <returns>The same instance of <see cref="ResultMessage"/>. for chaining.</returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="message"/> is null.
    /// </exception>
    public static ResultMessage WithStatus(this ResultMessage message, HttpStatusCode status) 
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));

        message.Status = status;
        return message;
    }

    /// <summary>
    /// <para>
    ///     Set a new value for the message exception.
    /// </para>
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The new exception.</param>
    /// <returns>The same instance of <see cref="ResultMessage"/>. for chaining.</returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="message"/> is null or <paramref name="exception"/> is null.
    /// </exception>
    public static ResultMessage WithException(this ResultMessage message, Exception exception)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));
        if (exception is null)
            throw new ArgumentNullException(nameof(exception));

        message.Exception = exception;
        return message;
    }
}
