
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace RoyalCode.OperationResults;

/// <summary>
/// Extension methods for serialize and deserialize <see cref="IOperationResult"/>.
/// </summary>
public static class SerializationExtensions
{
    /// <summary>
    /// Serialize a <see cref="IOperationResult"/> to a JSON string.
    /// </summary>
    /// <param name="result">The <see cref="IOperationResult"/> to be serialized.</param>
    /// <returns>The JSON string.</returns>
    public static string Serialize(this IOperationResult result) => SerializationContext.Serialize(result);

    /// <summary>
    /// Serialize a <see cref="IOperationResult"/> to a JSON string.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The <see cref="IOperationResult"/> to be serialized.</param>
    /// <returns>The JSON string.</returns>
    public static string Serialize<TValue>(this IOperationResult<TValue> result) => SerializationContext.Serialize(result);

    /// <summary>
    /// Deserialize a <see cref="OperationResult"/> from a JSON string.
    /// </summary>
    /// <param name="result">The <see cref="OperationResult"/> to be serialized.</param>
    /// <returns>The JSON string.</returns>
    public static string Serialize(this OperationResult result)
        => result.Match(
            static () => "{}",
            error => JsonSerializer.Serialize(error, SerializationContext.Default.AbstractOperationMessage));

    /// <summary>
    /// Serialize a <see cref="OperationResult{T}"/> para uma string JSON.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The <see cref="OperationResult{T}"/> to be serialized.</param>
    /// <returns>The JSON string.</returns>
    public static string Serialize<T>(this OperationResult<T> result)
       => result.Match(
            value => JsonSerializer.Serialize(value, SerializationContext.JsonSerializerOptions),
            error => JsonSerializer.Serialize(error, SerializationContext.Default.AbstractOperationMessage));

    /// <summary>
    /// Get the <see cref="JsonTypeInfo"/> for <see cref="ResultsCollection"/>.
    /// </summary>
    /// <param name="_">Used for extension methods.</param>
    /// <returns>
    ///     The <see cref="JsonTypeInfo"/> for <see cref="ResultsCollection"/>.
    /// </returns>
    public static JsonTypeInfo<ResultsCollection> GetJsonTypeInfo(this ResultsCollection _)
        => SerializationContext.Default.ResultsCollection;
}
