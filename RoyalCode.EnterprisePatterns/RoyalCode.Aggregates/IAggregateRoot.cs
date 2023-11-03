using RoyalCode.DomainEvents;
using RoyalCode.Entities;

namespace RoyalCode.Aggregates;

/// <summary>
/// <para>
///     A interface for identifying the aggregate root entity.
/// </para>
/// <para>
///     The aggregate root is the principal entity of an aggregate.
///     Generally there will only be repositories for the aggregate root entities,
///     and this entity will create and store the events occurred in all internal objects of the aggregate.
/// </para>
/// </summary>
public interface IAggregateRoot : IEntity, IHasEvents { }

/// <summary>
/// <para>
///     A interface for identifying the aggregate root entity, and define the ID type.
/// </para>
/// <para>
///     See also <see cref="IAggregateRoot"/>.
/// </para>
/// </summary>
/// <typeparam name="TId">The entity ID type.</typeparam>
public interface IAggregateRoot<out TId> : IAggregateRoot, IEntity<TId> { }