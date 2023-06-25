
using System.Text.Json.Serialization.Metadata;

namespace RoyalCode.OperationResults;

/// <summary>
/// A collection of results that can be deserialized.
/// </summary>
public static class DeserializableResultsCollection
{
    /// <summary>
    /// The <see cref="JsonTypeInfo"/> for <see cref="IEnumerable{ResultMessage}"/>.
    /// </summary>
    public static JsonTypeInfo<IEnumerable<ResultMessage>> ResultMessagesTypeInfo
        => SerializationContext.Default.ResultMessageCollection;
}