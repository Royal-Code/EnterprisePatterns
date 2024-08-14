using System.Text.Json;

namespace RoyalCode.Events.Outbox.Abstractions.Options;

/// <summary>
/// Outbox mechanism options.
/// </summary>
public class OutboxOptions
{
    /// <summary>
    /// The message types configured, with the serialisation metadata.
    /// </summary>
    public Dictionary<Type, TypeMetadata> Types { get; } = new();

    /// <summary>
    /// The standard serialisation options.
    /// </summary>
    public JsonSerializerOptions SerializerOptions { get; set; } = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <summary>
    /// Set up a new message type.
    /// </summary>
    /// <typeparam name="T">The type of message.</typeparam>
    /// <param name="typeName">Name of the message type.</param>
    /// <param name="options">Serialisation options for the type.</param>
    /// <returns>The same instance of <see cref="OutboxOptions"/>.</returns>
    public OutboxOptions AddMessageType<T>(string typeName, JsonSerializerOptions? options = null)
    {
        Types.Add(typeof(T), new TypeMetadata
        {
            PayloadType = typeof(T),
            TypeName = typeName,
            Version = 1,
            SerializerOptions = options,
        });

        return this;
    }
}
