using RoyalCode.OperationResults;

namespace RoyalCode.UnitOfWork.Abstractions;

/// <summary>
/// <para>
///     Result of the save operation, or result of the unit of work.
/// </para>
/// </summary>
[Obsolete("Something like the OperationResult struct must be created")]
public interface ISaveResult : IOperationResult
{
    /// <summary>
    /// <para>
    ///     Number of entities modified.
    /// </para>
    /// </summary>
    int Changes { get; }
}