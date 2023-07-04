namespace RoyalCode.OperationResults;

/// <summary>
/// Interface to results that have a value.
/// </summary>
[Obsolete]
public interface IResultHasValue
{
    /// <summary>
    /// The result value.
    /// </summary>
    object? Value { get; }
}