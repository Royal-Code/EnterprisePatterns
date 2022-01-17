using RoyalCode.OperationResult;

namespace RoyalCode.UnitOfWork.Abstractions;

/// <summary>
/// <para>
///     Result of the save operation, or result of the unit of work.
/// </para>
/// </summary>
public interface ISaveResult : IOperationResult
{
    /// <summary>
    /// <para>
    ///     Number of entities modified.
    /// </para>
    /// </summary>
    int Changes { get; }
}