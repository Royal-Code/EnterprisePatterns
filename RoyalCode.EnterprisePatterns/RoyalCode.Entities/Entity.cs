
namespace RoyalCode.Entities;

/// <summary>
/// <para>
///     Base implementation for <see cref="IEntity{TId}"/>
/// </para>
/// </summary>
/// <typeparam name="TId">The entity ID type.</typeparam>
public abstract class Entity<TId> : IEntity<TId>
{
    /// <summary>
    /// The entity ID type.
    /// </summary>
    public TId Id { get; protected set; }
}

/// <summary>
/// <para>
///     Base implementation for entities that have a code property.
/// </para>
/// </summary>
/// <typeparam name="TId">The entity ID type.</typeparam>
/// <typeparam name="TCode">The entity Code type.</typeparam>
public abstract class Entity<TId, TCode> : Entity<TId>, IHasCode<TCode>
{
    /// <summary>
    /// The entity Code.
    /// </summary>
    public TCode Codigo { get; protected set; }
}