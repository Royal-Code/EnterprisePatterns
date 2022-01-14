
namespace RoyalCode.Entities;

/// <summary>
/// <para>
///     Interface for entities that have a Guid type field.
/// </para>
/// <para>
///     This unique identifier field is not the same as the ID.
/// </para>
/// <para>
///     The purpose of this Guid property is to maintain a globally unique identifier of the same entity
///     distributed over several databases of different contexts.
///     Each database can have its structure and its Id type,
///     with different values for these IDs, however,
///     this Guid will maintain the same among all the databases.
/// </para>
/// </summary>
public interface IHasGuid
{
    /// <summary>
    /// The Guid of the entity.
    /// </summary>
    Guid Guid { get; }
}