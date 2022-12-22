
namespace RoyalCode.OperationResult;

/// <summary>
/// <para>
///     A operation result that aggregate many results.
/// </para>
/// </summary>
public interface IAggregateResult : IOperationResult
{
    /// <summary>
    /// <para>
    ///     Gets the inner results.
    /// </para>
    /// </summary>
    IEnumerable<IOperationResult> InnerResults { get; }
}