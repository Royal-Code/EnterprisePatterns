
namespace RoyalCode.Outbox.Abstractions.Models;

/// <summary>
/// <para>
///     Outbox message.
/// </para>
/// </summary>
public class OutboxMessage
{
    /// <summary>
    /// Creates a new message to be sent to the outbox.
    /// </summary>
    public OutboxMessage(string messageType, int versionType, string payload)
    {
        CreatedAt = DateTime.UtcNow;
        MessageType = messageType;
        VersionType = versionType;
        Payload = payload;
    }

#nullable disable
    /// <summary>
    /// Constructor for serialisation.
    /// </summary>
    public OutboxMessage() { }
#nullable restore

    /// <summary>
    /// Unique sequential id of the message.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Date the message was created or the date the event occurred.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Name representing the type of message.
    /// </summary>
    public string MessageType { get; set; }

    /// <summary>
    /// Message type version.
    /// </summary>
    public int VersionType { get; set; }

    /// <summary>
    /// Optional, the key that identifies the message, depending on the type of message.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Data from the serialised message object.
    /// </summary>
    public string Payload { get; set; }
}
