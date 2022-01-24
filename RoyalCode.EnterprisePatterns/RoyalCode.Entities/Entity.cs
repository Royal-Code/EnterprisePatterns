
namespace RoyalCode.Entities;

/// <summary>
/// <para>
///     Base implementation for <see cref="IEntity{TId}"/>
/// </para>
/// </summary>
/// <typeparam name="TId">The entity ID type.</typeparam>
public abstract class Entity<TId> : IEntity<TId>
{
#pragma warning disable CS8618

    /// <summary>
    /// The entity ID type.
    /// </summary>
    public TId Id { get; protected set; }

#pragma warning restore CS8618
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
#pragma warning disable CS8618

    /// <summary>
    /// The entity Code.
    /// </summary>
    public TCode Code { get; protected set; }

#pragma warning restore CS8618
}