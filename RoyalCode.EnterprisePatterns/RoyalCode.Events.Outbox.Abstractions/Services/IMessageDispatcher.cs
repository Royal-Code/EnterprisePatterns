namespace RoyalCode.Events.Outbox.Abstractions.Services;

/// <summary>
/// Message dispatch mechanisms.
/// </summary>
public interface IMessageDispatcher
{
    /// <summary>
    /// Dispatches a message asynchronously.
    /// </summary>
    /// <param name="message">The message instance.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A message processing task for the observer.</returns>
    Task DispatchAsync(object message, CancellationToken ct);
}
