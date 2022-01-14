
namespace RoyalCode.Entities;

/// <summary>
/// <para>
///     Interface to data models that contain an ID, usually entities, or DTO-style objects,
///     which reference the ID of an entity.
/// </para>
/// </summary>
public interface IHasId<TId>
{
    /// <summary>
    /// The ID of the data model.
    /// </summary>
    TId Id { get; }
}
