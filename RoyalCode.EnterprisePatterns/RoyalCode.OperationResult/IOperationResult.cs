using System.Text.Json.Serialization;

namespace RoyalCode.OperationResult;

/// <summary>
/// <para>
///     Interface to the operation result component, used as a return from a service or command.
/// </para>
/// </summary>
public interface IOperationResult
{
    /// <summary>
    /// Determine whether the result of the operation was success or failure.
    /// </summary>
    [JsonIgnore] 
    bool Success { get; }

    /// <summary>
    /// The result messages.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    IEnumerable<IResultMessage> Messages { get; }
}

/// <summary>
/// <para>
///     Interface to the operation result component, used as a return from a service or command.
/// </para>
/// <para>
///     This result type can have a value returned by the operation.
/// </para>
/// </summary>
/// <typeparam name="TValue">The result type of the value returned by the operation.</typeparam>
public interface IOperationResult<TValue> : IOperationResult, IResultHasValue
{
    /// <summary>
    /// The value returned by the operation.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    new TValue? Value { get; }
}
