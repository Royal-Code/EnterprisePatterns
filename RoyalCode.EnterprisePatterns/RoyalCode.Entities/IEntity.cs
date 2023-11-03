namespace RoyalCode.Entities;

/// <summary>
/// <para>
///     Interface for identifying entities.
/// </para>
/// </summary>
public interface IEntity { }

/// <summary>
/// <para>
///     Interface for identifying entities and the type of the ID.
/// </para>
/// </summary>
/// <typeparam name="TId">The entity ID type.</typeparam>
public interface IEntity<out TId> : IEntity, IHasId<TId> { }
