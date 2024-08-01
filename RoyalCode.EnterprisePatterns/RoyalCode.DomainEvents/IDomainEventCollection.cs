
namespace RoyalCode.DomainEvents;

/// <summary>
/// <para>
///     The domain event collection is an interface/component for storing domain events created by a aggregate.
///     Later, these events may be published/dispatched by other components.
/// </para>
/// <para>
///     Domain events will actually be published/dispatched by some component bound to the event collection,
///     usually when saving the context of the entity that created it, or by some application service
///     (or use case interactor).
/// </para>
/// </summary>
/// <remarks>
/// <para>
///     This component should be used by entities that are roots of aggregates.
///     These entities, which are roots of aggregates, shall implement IHasEvents.
/// </para>
/// </remarks>
public interface IDomainEventCollection : ICollection<IDomainEvent>
{
    /// <summary>
    /// <para>
    ///     Adds an observer to the event collection.
    /// </para>
    /// <para>
    ///     The Observer will receive all events added to the collection.
    ///     If there are already events added to the collection when the observer is added,
    ///     the events will be sent to the observer, one by one, when the observer is added.
    ///     Subsequent events will be passed to the observer when the event is added.
    /// </para>
    /// </summary>
    /// <param name="observerAction">The observer action.</param>
    void Observe(Action<IDomainEvent> observerAction);
    
    /// <summary>
    /// <para>
    ///     Removes an observer from the event collection.
    /// </para>
    /// </summary>
    /// <param name="observerAction">The observer action.</param>
    void RemoveObserver(Action<IDomainEvent> observerAction);
}