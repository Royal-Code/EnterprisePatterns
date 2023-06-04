
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace RoyalCode.OperationResults;

/// <summary>
/// A collection of results that can be deserialized.
/// </summary>
public readonly struct DeserializableResultsCollection
{
    /// <summary>
    /// Options for serialization and deserialization.
    /// </summary>
    public static JsonTypeInfo<DeserializableResultsCollection> JsonTypeInfo 
        => SerializationContext.Default.DeserializableResultsCollection;

    /// <summary>
    /// Initialize the <see cref="DeserializableResultsCollection"/>.
    /// </summary>
    /// <param name="messages"></param>
    [JsonConstructor]
    public DeserializableResultsCollection(ResultsCollection? messages)
    {
        Messages = messages;
    }

    /// <summary>
    /// The messages.
    /// </summary>
    public ResultsCollection? Messages { get; }
}
