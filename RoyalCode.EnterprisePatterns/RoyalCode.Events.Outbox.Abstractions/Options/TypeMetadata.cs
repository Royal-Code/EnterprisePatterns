using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace RoyalCode.Events.Outbox.Abstractions.Options;

/// <summary>
/// Metadata for a particular type of message sent to the Outbox.
/// </summary>
public class TypeMetadata
{
    /// <summary>
    /// The type of the message payload. Represents the outbox message object.
    /// </summary>
    public required Type PayloadType { get; init; }

    /// <summary>
    /// The name for the message type, which is stored together with the outbox message.
    /// </summary>
    public required string TypeName { get; init; }

    /// <summary>
    /// The version of the object that represents the message, or version of the payload.
    /// </summary>
    public required int Version { get; init; }

    /// <summary>
    /// Optional serialisation options.
    /// </summary>
    public JsonSerializerOptions? SerializerOptions { get; set; }

    /// <summary>
    /// Optional, serialization json type.
    /// </summary>
    public JsonTypeInfo? JsonTypeInfo { get; set; }
}