using RoyalCode.DomainEvents;
using RoyalCode.DomainEvents.Attributes;
using System.Collections.Concurrent;
using System.Reflection;

namespace RoyalCode.Persistence.EntityFramework.Events.Entity;

/// <summary>
/// <para>
///     Static class that store the event names for the domain events types.
/// </para>
/// </summary>
public static class DomainEventNames
{
    private static readonly ConcurrentDictionary<Type, string> eventNames = new();

    /// <summary>
    /// Get the event name based on the event type.
    /// </summary>
    /// <param name="eventType">The event type.</param>
    /// <returns>The event name.</returns>
    public static string GetName(Type eventType)
    {
        return eventNames.GetOrAdd(eventType, et =>
        {
            return et.GetCustomAttribute<EventNameAttribute>()?.Name ?? eventType.Name;
        });
    }

    /// <summary>
    /// Set the event name for the event type.
    /// </summary>
    /// <param name="eventType">The event type.</param>
    /// <param name="eventName">The event name.</param>
    public static void SetEventName(Type eventType, string eventName)
    {
        if (!eventType.IsAssignableTo(typeof(IDomainEvent)))
            throw new ArgumentException("The event type must be assignable to IDomainEvent.", nameof(eventType));

        eventNames.AddOrUpdate(eventType, eventName, (_, _) => eventName);
    }

    /// <summary>
    /// Set the event name for the event type.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="name">The event name.</param>
    public static void SetEventName<TEvent>(string name)
        where TEvent : IDomainEvent
    {
        SetEventName(typeof(TEvent), name);
    }
}