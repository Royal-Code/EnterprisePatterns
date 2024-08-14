using RoyalCode.SmartProblems;

namespace RoyalCode.Events.Outbox.Abstractions.Contracts.Handlers;

/// <summary>
/// Handler for creating messages in the outbox.
/// </summary>
public interface ICreateMessageHandler
{
    /// <summary>
    /// Creates a new message record in the outbox.
    /// </summary>
    /// <param name="request">Request for message creation.</param>
    /// <returns>The result of the operation.</returns>
    public Result Handle(CreateMessage request);
}
