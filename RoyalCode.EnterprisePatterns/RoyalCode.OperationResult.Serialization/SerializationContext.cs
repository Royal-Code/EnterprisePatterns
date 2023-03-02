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
[JsonSerializable(typeof(IOperationResult), TypeInfoPropertyName = "AbstractOperationMessage")]
[JsonSerializable(typeof(IResultMessage), TypeInfoPropertyName = "AbstractResultMessage")]
[JsonSerializable(typeof(BaseResult))]
[JsonSerializable(typeof(DeserializableResult))]
[JsonSerializable(typeof(ResultMessage))]
internal partial class SerializationContext : JsonSerializerContext
{
    /// <summary>
    /// The default <see cref="JsonSerializerOptions"/> used for serialization and deserialization.
    /// </summary>
    public static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// The default <see cref="JsonSerializerOptions"/> used for serialization and deserialization.
    /// </summary>
    public static readonly JsonSerializerOptions IndentedJsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
    };

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

    /// <summary>
    /// Serialize a <see cref="IOperationResult"/> to a JSON string.
    /// </summary>
    /// <param name="result">The <see cref="IOperationResult"/> to be serialized.</param>
    /// <returns>The JSON string.</returns>
    public static string Serialize(IOperationResult result)
        => JsonSerializer.Serialize(result, Default.AbstractOperationMessage);

    /// <summary>
    /// Serialize a <see cref="IOperationResult"/> to a JSON string.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The <see cref="IOperationResult"/> to be serialized.</param>
    /// <returns>The JSON string.</returns>
    public static string Serialize<TValue>(IOperationResult<TValue> result)
        => JsonSerializer.Serialize(result, JsonSerializerOptions);

    /// <summary>
    /// Serialize a <see cref="RoyalCode.OperationResult.BaseResult"/> to a JSON string.
    /// </summary>
    /// <param name="result">The <see cref="RoyalCode.OperationResult.BaseResult"/> to be serialized.</param>
    /// <returns>The JSON string.</returns>
    public static string Serialize(BaseResult result)
        => JsonSerializer.Serialize(result, Default.BaseResult);

    /// <summary>
    /// Serialize a <see cref="ValueResult{TValue}"/> to a JSON string.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The <see cref="ValueResult{TValue}"/> to be serialized.</param>
    /// <returns>The JSON string.</returns>
    public static string Serialize<TValue>(ValueResult<TValue> result)
        => JsonSerializer.Serialize(result, JsonSerializerOptions);
}
