namespace RoyalCode.OperationResult;

/// <summary>
/// Interface to results that have a value.
/// </summary>
public interface IResultHasValue
{
    /// <summary>
    /// The result value.
    /// </summary>
    object? Value { get; }
}