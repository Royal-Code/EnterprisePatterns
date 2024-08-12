using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RoyalCode.Repositories.Abstractions;

/// <summary>
/// Type to define the identity of an entity.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
/// <typeparam name="TId">Type of identity value.</typeparam>
public readonly struct Id<TEntity, TId>
    where TEntity : class
{
    /// <summary>
    /// Implicit operator to convert the identity value to the identity type.
    /// </summary>
    /// <param name="id"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Id<TEntity, TId>(TId id) => new(id);

    /// <summary>
    /// Creates new value for the organisation's identity.
    /// </summary>
    /// <param name="value"></param>
    public Id(TId value) => Value = value;

    /// <summary>
    /// The value of the entity's identity.
    /// </summary>
    public TId Value { get; }

    /// <summary>
    /// String parse for the entity's identity type.
    /// </summary>
    /// <param name="input">Input, in string format.</param>
    /// <param name="id">Output, in the entity's identity type.</param>
    /// <returns>True if the parse was possible, false otherwise.</returns>
    public static bool TryParse(string? input, out Id<TEntity, TId> id)
    {
        if (input == null)
        {
            id = default;
            return true;
        }

        try
        {
            var converter = TypeDescriptor.GetConverter(typeof(TId));
            if (converter is not null && converter.CanConvertFrom(typeof(string)))
            {
                id = new Id<TEntity, TId>((TId)converter.ConvertFromString(input)!);
                return true;
            }
        }
        catch { /* Ignore exceptions, return false at the end. */ }

        id = default;
        return false;
    }
}
