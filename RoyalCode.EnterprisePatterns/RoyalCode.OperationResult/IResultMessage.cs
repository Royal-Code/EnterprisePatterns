using System.Net;

namespace RoyalCode.OperationResult;

/// <summary>
/// <para>
///     Interface for the operation results messages.
/// </para>
/// </summary>
public interface IResultMessage
{
    /// <summary>
    /// The message type.
    /// </summary>
    ResultMessageType Type { get; }

    /// <summary>
    /// The text of the message. The object is to display the message to users.
    /// </summary>
    string Text { get; }

    /// <summary>
    /// Property that originated the message, 
    /// or property that is intended for the message. 
    /// When not linked to any property, inform a blank string.
    /// </summary>
    string? Property { get; }

    /// <summary>
    /// Some kind of code that can identify the type of message or the message itself.
    /// </summary>
    string? Code { get; }

    /// <summary>
    /// <para>
    ///     Contains the values of status codes defined for HTTP.
    /// </para>
    /// <para>
    ///     This is a optional property.
    /// </para>
    /// <para>
    ///     Used to help convert operation results into webapi's responses.
    /// </para>
    /// </summary>
    HttpStatusCode? HttpStatus { get; }

    /// <summary>
    /// Exception related to the message.
    /// </summary>
    ResultMessageException? Exception { get; }
}
