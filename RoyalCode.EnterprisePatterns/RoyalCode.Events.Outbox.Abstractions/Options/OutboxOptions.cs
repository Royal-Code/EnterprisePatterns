using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace RoyalCode.Events.Outbox.Abstractions.Options;

/// <summary>
/// Outbox mechanism options.
/// </summary>
public class OutboxOptions
{
    /// <summary>
    /// The message types configured, with the serialisation metadata.
    /// </summary>
    public Dictionary<Type, TypeMetadata> Types { get; } = [];

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

    /// <summary>
    /// Set up a new message type.
    /// </summary>
    /// <typeparam name="T">The type of message.</typeparam>
    /// <param name="typeName">Name of the message type.</param>
    /// <param name="typeInfo">Json type info for serialization.</param>
    /// <returns>The same instance of <see cref="OutboxOptions"/>.</returns>
    public OutboxOptions AddMessageType<T>(string typeName, JsonTypeInfo? typeInfo)
    {
        Types.Add(typeof(T), new TypeMetadata
        {
            PayloadType = typeof(T),
            TypeName = typeName,
            Version = 1,
            JsonTypeInfo = typeInfo,
        });

        return this;
    }

    /// <summary>
    /// Try to get the metadata of a type.
    /// </summary>
    /// <typeparam name="T">The type of event.</typeparam>
    /// <param name="metadata">Output, metadata.</param>
    /// <returns>True if it exists, false if it is not configured.</returns>
    public bool TryGetMetadata<T>([NotNullWhen(true)] out TypeMetadata? metadata)
    {
        return Types.TryGetValue(typeof(T), out metadata);
    }

    /// <summary>
    /// Try to get the metadata of a type.
    /// </summary>
    /// <param name="type">The type of event.</param>
    /// <param name="metadata">Output, metadata.</param>
    /// <returns>True if it exists, false if it is not configured.</returns>
    public bool TryGetMetadata(Type type, [NotNullWhen(true)] out TypeMetadata? metadata)
    {
        return Types.TryGetValue(type, out metadata);
    }
}
