namespace RoyalCode.Events.Outbox.Abstractions.Contracts;

/// <summary>
/// Request to inform Outbox of the last message successfully read.
/// </summary>
public sealed class CommitConsumed
{
    /// <summary>
    /// The consumer's identifying name.
    /// </summary>
    public required string ConsumerName { get; init; }

    /// <summary>
    /// The Id of the last message successfully read.
    /// </summary>
    public long LastConsumedMessageId { get; init; }
}
