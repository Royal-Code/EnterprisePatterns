namespace RoyalCode.DomainEvents;

/// <summary>
/// <para>
///     Abstract implementation for domain events.
/// </para>
/// </summary>
public abstract class DomainEventBase : IDomainEvent
{
    /// <summary>
    /// <para>
    ///     Creates a new event by generating a new identifier and using the current date and time
    ///     to determine when the event occurred.
    /// </para>
    /// </summary>
    protected DomainEventBase()
    {
        Id = Guid.NewGuid();
        Occurred = DateTimeOffset.Now;
    }

    /// <summary>
    /// <para>
    ///     Creates a new instance of the event determining the Id and when it occurred.
    ///     Normally used for deserialisation.
    /// </para>
    /// </summary>
    /// <param name="id">The event ID.</param>
    /// <param name="occurred">When the event occurred.</param>
    protected DomainEventBase(Guid id, DateTimeOffset occurred)
    {
        Id = id;
        Occurred = occurred;
    }

    /// <summary>
    /// The event ID.
    /// </summary>
    public Guid Id { get; }
    
    /// <summary>
    /// When the event occurred.
    /// </summary>
    public DateTimeOffset Occurred { get; }
}