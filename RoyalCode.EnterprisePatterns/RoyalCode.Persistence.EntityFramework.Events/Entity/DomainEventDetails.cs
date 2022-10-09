using RoyalCode.DomainEvents;
using RoyalCode.Entities;
using RoyalCode.Persistence.EntityFramework.Events.Extensions;
using System.Text.Json;

namespace RoyalCode.Persistence.EntityFramework.Events.Entity
{
    /// <summary>
    /// <para>
    ///     A generic entity to store domain events.
    /// </para>
    /// </summary>
    public class DomainEventDetails : Entity<Guid>
    {
        /// <summary>
        /// Creates a new entity from the domain event;
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        public DomainEventDetails(IDomainEvent domainEvent)
        {
            if (domainEvent is null)
                throw new ArgumentNullException(nameof(domainEvent));

            Id = domainEvent.Id;
            Occurred = domainEvent.Occurred;
            TypeFullName = domainEvent.GetType().FullNameWithAssembly();
            Json = JsonSerializer.Serialize(domainEvent, domainEvent.GetType());
        }

#pragma warning disable CS8618
        
        /// <summary>
        /// Constructor for deserialization.
        /// </summary>
        protected DomainEventDetails() { }

#pragma warning restore CS8618

        /// <summary>
        /// When the domain event occurs.
        /// </summary>
        public DateTimeOffset Occurred { get; }

        /// <summary>
        /// The fullname of the domain event type.
        /// </summary>
        public string TypeFullName { get; }

        /// <summary>
        /// Json value of the domain event.
        /// </summary>
        public string Json { get; }

        /// <summary>
        /// Gets the type of the serialized event.
        /// </summary>
        /// <returns>Type, or null if no type exists in the loaded Assemblies.</returns>
        public Type? DomainEventType => Type.GetType(TypeFullName);

        /// <summary>
        /// Deserializes the Json to the serialized event type.
        /// </summary>
        /// <returns>An instance of domain event.</returns>
        public IDomainEvent? Deserialize() => (IDomainEvent?)JsonSerializer.Deserialize(Json, DomainEventType!);

        /// <summary>
        /// Deserializes the event Json to a given type.
        /// </summary>
        /// <typeparam name="TEvent">The domain event type to be deserialized.</typeparam>
        /// <returns>An instance of <typeparamref name="TEvent"/>.</returns>
        public TEvent? Deserialize<TEvent>()
            where TEvent : class, IDomainEvent
            => JsonSerializer.Deserialize<TEvent>(Json);
    }
}
