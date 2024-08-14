using RoyalCode.Events.Outbox.Abstractions.Models;

namespace RoyalCode.Events.Outbox.Abstractions.Contracts.Responses;

/// <summary>
/// Response to the search for messages from an outbox consumer.
/// </summary>
public sealed class RetrievedMessages
{
    /// <summary>
    /// Messages that have been read.
    /// </summary>
    public required IEnumerable<OutboxMessage> Messages { get; init; }

    /// <summary>
    /// Total messages read.
    /// </summary>
    public required int Count { get; init; }

    /// <summary>
    /// If there are more messages to read.
    /// </summary>
    public required bool HasMore { get; init; }
}