
using RoyalCode.OperationResult.Exceptions;

namespace RoyalCode.OperationResult;

/// <summary>
/// Extension methods for results and messages.
/// </summary>
public static class MessageExtensions
{
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
        return message.Exception?.GetOriginExcepion() ?? message.ToInvalidOperationException();
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
        var originalException = message.Exception?.GetOriginExcepion();
        return originalException is not null
            ? originalException is InvalidOperationException ioex
                ? ioex
                : new InvalidOperationException(message.Text, originalException)
            : message.Exception is not null
                ? new InvalidOperationException(message.Text, message.Exception.ToInvalidOperationInnerException())
                : new InvalidOperationException(message.Text);
    }

    /// <summary>
    /// <para>
    ///     Generate one <see cref="InvalidOperationInnerException"/> from the message exception model.
    /// </para>
    /// </summary>
    /// <param name="messageException">The message exception model.</param>
    /// <returns>
    /// <para>
    ///     A new instance of <see cref="InvalidOperationInnerException"/>.
    /// </para>
    /// </returns>
    public static InvalidOperationInnerException ToInvalidOperationInnerException(this ResultMessageException messageException)
    {
        return messageException.InnerException is not null
            ? new InvalidOperationInnerException(
                messageException.Message,
                messageException.StackTrace!,
                messageException.FullNameOfExceptionType,
                messageException.InnerException.ToInvalidOperationInnerException())
            : new InvalidOperationInnerException(
                messageException.Message,
                messageException.StackTrace!,
                messageException.FullNameOfExceptionType);
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
}
