
namespace RoyalCode.Entities;

/// <summary>
/// <para>
///     Interface to data models with a code property.
/// </para>
/// <para>
///     The code is not the entity ID, but a unique identifier, usually more human-friendly.
/// </para>
/// </summary>
/// <typeparam name="TCode">Data type of the code.</typeparam>
public interface IHasCode<out TCode>
{
    /// <summary>
    /// The code of the data model.
    /// </summary>
    TCode Code { get; }
}