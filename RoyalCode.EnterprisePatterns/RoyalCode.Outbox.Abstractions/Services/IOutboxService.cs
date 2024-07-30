
using RoyalCode.Outbox.Abstractions.Models;

namespace RoyalCode.Outbox.Abstractions.Services;

/// <summary>
/// Service that converts messages to the outbox and dispatch to consumers.
/// </summary>
public interface IOutboxService
{
    /// <summary>
    /// <para>
    ///     Creates a <see cref="OutboxMessage"/> entity from a message and adds it to the outbox.
    /// </para>
    /// <para>
    ///     The changes are not saved, you need to apply SaveChanges after execution.
    /// </para>
    /// </summary>
    /// <param name="message">The message to be persisted.</param>
    /// <exception cref="InvalidOperationException">
    ///     If the message type has not been configured, or an error occurs when persisting the message.
    /// </exception>
    void Write(object message);

    /// <summary>
    /// <para>
    ///     It deserialises the messages obtained from the outbox and sends them to the respective handlers.
    /// </para>
    /// <para>
    ///     To consume dispatched messages, you need to implement <see cref="IMessageObserver{TMessage}"/>.
    /// </para>
    /// </summary>
    /// <param name="messages">A collection of messagens to be dispatched.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task.</returns>
    Task DispatchAsync(IEnumerable<OutboxMessage> messages, CancellationToken ct);
}
