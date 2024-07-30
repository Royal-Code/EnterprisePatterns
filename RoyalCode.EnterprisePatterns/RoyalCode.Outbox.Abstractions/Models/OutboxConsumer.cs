namespace RoyalCode.Outbox.Abstractions.Models;

/// <summary>
/// Outbox consumer.
/// </summary>
public class OutboxConsumer
{
    /// <summary>
    /// It creates a new consumer.
    /// </summary>
    /// <param name="name">Unique name of the consumer.</param>
    public OutboxConsumer(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
        LastConsumedMessageId = 0;
    }

#nullable disable
    /// <summary>
    /// Constructor for serialisation.
    /// </summary>
    public OutboxConsumer() { }
#nullable restore

    /// <summary>
    /// Unique consumer identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The consumer's name is also a unique identifier.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Id of the last message consumed.
    /// </summary>
    public long LastConsumedMessageId { get; set; }
}

