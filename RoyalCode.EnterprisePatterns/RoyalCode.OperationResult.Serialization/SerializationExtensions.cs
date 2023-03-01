
namespace RoyalCode.OperationResult;

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
}
