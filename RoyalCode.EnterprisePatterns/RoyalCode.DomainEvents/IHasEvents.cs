
namespace RoyalCode.DomainEvents;

/// <summary>
/// <para>
///     Interface for aggregator root that creates domain events.
/// </para>
/// <para>
///     The events will be stored in a <see cref="IDomainEventCollection"/>,
///     which will make the events available for the work unit, or event dispatch components,
///     to trigger them during finalisation (saving changes).
/// </para>
/// </summary>
public interface IHasEvents
{
    /// <summary>
    ///     The event collection, which will contain the events created by the entity during the work unit.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Domain events will actually be published/dispatched by some component bound to the event collection,
    ///     usually when saving the context of the entity that created it, or by some application service
    ///     (or use case interactor).
    /// </para>
    /// </remarks>
    IDomainEventCollection? DomainEvents { get; set; }
}
