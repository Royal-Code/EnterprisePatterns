namespace RoyalCode.Outbox.Abstractions.Services;

/// <summary>
/// <para>
///     Message subscription.
/// </para>
/// <para>
///     Interface for a component that observes messages read from the outbox.
/// </para>
/// </summary>
/// <typeparam name="TMessage">O tipo da mensagem a ser observada.</typeparam>
public interface IMessageObserver<in TMessage>
{
    /// <summary>
    /// Handles a message read from the outbox.
    /// </summary>
    /// <param name="message">The message to be manipulated.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A message handling execution task.</returns>
    public Task HandleAsync(TMessage message, CancellationToken ct);
}
