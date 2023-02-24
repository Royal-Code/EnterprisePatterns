
using RoyalCode.OperationResult.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RoyalCode.OperationResult;

/// <summary>
/// Serialization context for <see cref="IOperationResult"/>.
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(BaseResult))]
[JsonSerializable(typeof(DeserializableResult))]
[JsonSerializable(typeof(ResultMessage))]
public partial class ResultsSerializeContext : JsonSerializerContext
{
    /// <summary>
    /// The default <see cref="JsonSerializerOptions"/> used for serialization and deserialization.
    /// </summary>
    public static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };
    
    /// <summary>
    /// Deserialize a <see cref="OperationResult.BaseResult"/> from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string.</param>
    /// <returns>The deserialized <see cref="OperationResult.BaseResult"/>.</returns>
    public static BaseResult? DeserializeBaseResult(string json)
        => OperationResult.BaseResult.Deserialize(json);

    /// <summary>
    /// Deserialize a <see cref="IOperationResult"/> from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string.</param>
    /// <returns>The deserialized <see cref="IOperationResult"/>.</returns>
    public static IOperationResult? Deserialize(string json)
        => JsonSerializer.Deserialize(json, Default.DeserializableResult);

    /// <summary>
    /// Deserialize a <see cref="IOperationResult"/> from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string.</param>
    /// <returns>The deserialized <see cref="IOperationResult"/>.</returns>
    public static IOperationResult<TValue>? Deserialize<TValue>(string json)
        => JsonSerializer.Deserialize<DeserializableResult<TValue>>(json, JsonSerializerOptions);
}
