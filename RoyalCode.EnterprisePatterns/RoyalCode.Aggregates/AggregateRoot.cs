// Ignore Spelling: evt

using RoyalCode.DomainEvents;
using RoyalCode.Entities;

namespace RoyalCode.Aggregates;

/// <summary>
/// <para>
///     Base implementation for <see cref="IAggregateRoot{TId}"/>.
/// </para>
/// </summary>
/// <typeparam name="TId">The entity ID type.</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot<TId>
{
    /// <summary>
    /// The domain event collection.
    /// </summary>
    public IDomainEventCollection? DomainEvents { get; set; }

    /// <summary>
    /// <para>
    ///     Adds a domain event to the collection, validating that the collection is not null
    ///     and creating a collection if necessary.
    /// </para>
    /// </summary>
    /// <param name="evt">The domain event to be added to the domain event collection.</param>
    protected void AddEvent(IDomainEvent evt)
    {
        DomainEvents ??= new DomainEventCollection();
        DomainEvents.Add(evt);
    }
}

/// <summary>
/// <para>
///     Basic implementation for <see cref="IAggregateRoot{TId}"/>, where the entity has a code property.
/// </para>
/// </summary>
/// <typeparam name="TId">The entity ID type.</typeparam>
/// <typeparam name="TCode">The entity Code type.</typeparam>
public abstract class AggregateRoot<TId, TCode> : AggregateRoot<TId>, IHasCode<TCode>
{
#pragma warning disable CS8618

    /// <summary>
    /// The code of the entity.
    /// </summary>
    public TCode Code { get; protected set; }

#pragma warning restore CS8618
}
