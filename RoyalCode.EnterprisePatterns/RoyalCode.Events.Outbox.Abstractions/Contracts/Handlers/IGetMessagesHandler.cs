using RoyalCode.Events.Outbox.Abstractions.Contracts.Responses;
using RoyalCode.SmartProblems;

namespace RoyalCode.Events.Outbox.Abstractions.Contracts;

/// <summary>
/// Executes the request to obtain messages from the outbox for a given consumer.
/// </summary>
public interface IGetMessagesHandler
{
    /// <summary>
    /// Executes the request to obtain messages from the outbox for a given consumer.
    /// </summary>
    /// <param name="request">Request to obtain messages from the outbox.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    ///     The Result with the messages, if any, or the problem that occurred.
    /// </returns>
    public Task<Result<RetrievedMessages>> HandleAsync(GetMessages request, CancellationToken ct);
}
