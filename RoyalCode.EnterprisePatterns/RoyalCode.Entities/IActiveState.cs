
namespace RoyalCode.Entities;

/// <summary>
/// <para>
///     Interface to data models that has an active state property, denoting whether the entity is active or not.
/// </para>
/// </summary>
public interface IActiveState
{
    /// <summary>
    /// <para>
    ///     Determines whether the entity is active or not.
    /// </para>
    /// </summary>
    bool IsActive { get; }
}
