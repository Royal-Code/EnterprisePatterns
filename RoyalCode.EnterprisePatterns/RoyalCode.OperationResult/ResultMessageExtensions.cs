
using System.Net;

namespace RoyalCode.OperationResults;

/// <summary>
/// Extension methods for results and messages.
/// </summary>
public static class ResultMessageExtensions
{
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
