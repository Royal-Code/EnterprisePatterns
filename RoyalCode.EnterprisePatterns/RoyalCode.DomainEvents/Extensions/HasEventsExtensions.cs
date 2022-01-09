using RoyalCode.DomainEvents;

// ReSharper disable CheckNamespace

namespace HexaSamples.SeedWork.Entities;

/// <summary>
/// <para>
///     Extensions methods for <see cref="IHasEvents"/>.
/// </para>
/// </summary>
public static class HasEventsExtensions
{
    /// <summary>
    /// <para>
    ///     Checks if an event of a certain type exists.
    /// </para>
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="hasEvents">An object that contains the domain event collection.</param>
    /// <returns>
    /// <para>
    ///     True if exists, otherwise false.
    /// </para>
    /// </returns>
    public static bool HasEvent<TEvent>(this IHasEvents hasEvents)
        where TEvent : class, IDomainEvent
    {
        if (hasEvents is null)
            throw new ArgumentNullException(nameof(hasEvents));

        return hasEvents.DomainEvents?.OfType<TEvent>().Any() ?? false;
    }

    /// <summary>
    /// <para>
    ///     Try get the first evento for a given type.
    /// </para>
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="hasEvents">An object that contains the domain event collection.</param>
    /// <param name="event">The event instance, if exists.</param>
    /// <returns>
    /// <para>
    ///     True if find some event for the given type, otherwise false.
    /// </para>
    /// </returns>
    public static bool TryGetEvent<TEvent>(this IHasEvents hasEvents, out TEvent? @event)
        where TEvent : class, IDomainEvent
    {
        if (hasEvents is null)
            throw new ArgumentNullException(nameof(hasEvents));

        @event = hasEvents.DomainEvents?.OfType<TEvent>().FirstOrDefault();
        return @event != null;
    }

    /// <summary>
    /// <para>
    ///     Get all events for a given type.
    /// </para>
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="hasEvents">An object that contains the domain event collection.</param>
    /// <returns>
    /// <para>
    ///     An <see cref="IEnumerable{T}"/> of the events.
    /// </para>
    /// </returns>
    public static IEnumerable<TEvent> GetEvents<TEvent>(this IHasEvents hasEvents)
        where TEvent : class, IDomainEvent
    {
        if (hasEvents is null)
            throw new ArgumentNullException(nameof(hasEvents));

        return hasEvents.DomainEvents?.OfType<TEvent>() ?? Array.Empty<TEvent>();
    }

    /// <summary>
    /// <para>
    ///     try get all events for a given type.
    /// </para>
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="hasEvents">An object that contains the domain event collection.</param>
    /// <param name="events">An <see cref="IEnumerable{T}"/> of the events.</param>
    /// <returns>
    /// <para>
    ///     True if find some event for the given type, otherwise false.
    /// </para>
    /// </returns>
    public static bool TryGetEvents<TEvent>(this IHasEvents hasEvents, out IEnumerable<TEvent> events)
        where TEvent : class, IDomainEvent
    {
        if (hasEvents is null)
            throw new ArgumentNullException(nameof(hasEvents));

        events = hasEvents.DomainEvents?.OfType<TEvent>() ?? Array.Empty<TEvent>();
        return events.Any();
    }
}