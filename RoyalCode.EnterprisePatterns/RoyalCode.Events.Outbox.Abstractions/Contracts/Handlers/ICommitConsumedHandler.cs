using RoyalCode.SmartProblems;

namespace RoyalCode.Events.Outbox.Abstractions.Contracts.Handlers;

/// <summary>
/// Handler to inform Outbox of the last message successfully read by the consumer.
/// </summary>
public interface ICommitConsumedHandler
{
    /// <summary>
    /// Executes the request to inform Outbox of the last message successfully read by the consumer.
    /// </summary>
    /// <param name="request">The request to inform Outbox of the last message successfully read by the consumer.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The operation result.</returns>
    Task<Result> HandleAsync(CommitConsumed request, CancellationToken ct);
}
