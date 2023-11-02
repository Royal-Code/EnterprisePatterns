
namespace RoyalCode.Entities;

/// <summary>
/// <para>
///     Interface to data models that can be soft-deleted.
/// </para>
/// <para>
///     Soft-deleted entities are entities that are not physically deleted from the database,
///     but are marked as deleted by setting the <see cref="IsDeleted"/> property to <c>true</c>.
/// </para>
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// <para>
    ///     Determines whether the entity is deleted or not.
    /// </para>
    /// </summary>
    bool IsDeleted { get; }
}
