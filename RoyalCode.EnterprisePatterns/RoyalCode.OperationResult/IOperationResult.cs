using System.Text.Json.Serialization;

namespace RoyalCode.OperationResults;

/// <summary>
/// <para>
///     Interface to the operation result component, used as a return from a service or command.
/// </para>
/// </summary>
[Obsolete]
public interface IOperationResult
{
    /// <summary>
    /// Determine whether the result of the operation was success or failure.
    /// </summary>
    [JsonIgnore] 
    bool Success { get; }

    /// <summary>
    /// Determines whether the result of the operation was success or failure.
    /// </summary>
    [JsonIgnore]
    bool Failure => !Success;

    /// <summary>
    /// Count of the error messages of the result.
    /// </summary>
    [JsonIgnore]
    int ErrorsCount { get; }

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
[Obsolete]
public interface IOperationResult<out TValue> : IOperationResult, IResultHasValue
{
    /// <summary>
    /// The value returned by the operation.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    new TValue? Value { get; }
}
