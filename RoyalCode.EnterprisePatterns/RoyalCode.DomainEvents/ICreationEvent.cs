namespace RoyalCode.DomainEvents;

/// <summary>
/// <para>
///     This interface is additional to the domain events.
///     It is used for events of aggregate or entity creation,
///     where the ID of the entity created is generated in the database and the event will use this ID.
/// </para>
/// <para>
///     The component which will handle the events, linked to the unit of work,
///     must inform the events which implement this interface after the entities are saved in the database,
///     so that the event can extract the entity's ID.
/// </para>
/// <para>
///     Only after this notification can events be dispatched via messaging systems.  
/// </para>
/// <para>
///     If events are stored in the database, they should be stored after notification.
///     If the database supports transactions, it can be done in two steps with an open transaction,
///     where you first record the entities, and after notifying the creation events,
///     record the events in the database, finalizing the transaction.
/// </para>
/// </summary>
public interface ICreationEvent
{
    /// <summary>
    /// Notified after entities are saved and have the ID generated by the database.
    /// </summary>
    void Saved();
}