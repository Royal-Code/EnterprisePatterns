using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RoyalCode.Commands.Handlers;

/// <summary>
/// <para>
///     Provides a cache of display names for types.
/// </para>
/// </summary>
public static class DisplayNames
{
    private static readonly ConcurrentDictionary<Type, string> cache = new();

    /// <summary>
    /// <para>
    ///     Gets or sets the function that produces the display name for a type.
    /// </para>
    /// </summary>
    public static Func<Type, string> ProduceDisplayName { get; set; } = InternalProduceDisplayName;

    /// <summary>
    /// <para>
    ///     Gets the display name for the specified type.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <returns>The display name.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Get<T>() => ProduceDisplayName(typeof(T));

    /// <summary>
    /// <para>
    ///     Default implementation of <see cref="ProduceDisplayName"/>.
    /// </para>
    /// </summary>
    /// <param name="arg">The type.</param>
    /// <returns>The display name.</returns>
    private static string InternalProduceDisplayName(Type arg)
    {
        // check cache
        if (cache.TryGetValue(arg, out var displayName))
            return displayName;

        // get display name
        var displayNameAttribute = arg.GetCustomAttribute<DisplayNameAttribute>();
        if (displayNameAttribute is not null)
        {
            displayName = displayNameAttribute.DisplayName;
            cache.TryAdd(arg, displayName);
            return displayName;
        }

        // get name
        displayName = arg.Name;
        cache.TryAdd(arg, displayName);
        return displayName;
    }
}