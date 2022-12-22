
namespace RoyalCode.OperationResult;

/// <inheritdoc />
public class AggregateResult : IAggregateResult
{
    private readonly LinkedList<IOperationResult> innerResults = new();

    /// <inheritdoc />
    public IEnumerable<IOperationResult> InnerResults => innerResults;

    /// <summary>
    /// <para>
    ///     Determine whether the result of all the operations was success.
    ///     When one operation fail, the result is fail, and the value will be false.
    /// </para>
    /// </summary>
    public bool Success => innerResults.All(r => r.Success);

    /// <summary>
    /// <para>
    ///     Get all messages from all inner results.
    /// </para>
    /// </summary>
    public IEnumerable<IResultMessage> Messages => innerResults.SelectMany(r => r.Messages);

    /// <summary>
    /// Add a inner result.
    /// </summary>
    /// <param name="result">The inner result.</param>
    public void AddResult(IOperationResult result) => innerResults.AddLast(result);
}