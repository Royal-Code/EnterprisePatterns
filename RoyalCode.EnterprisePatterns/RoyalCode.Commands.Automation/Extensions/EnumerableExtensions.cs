﻿using System.Diagnostics.CodeAnalysis;

namespace RoyalCode.Commands.Abstractions;

/// <summary>
/// Extension methods for <see cref="IEnumerable{T}"/>.
/// </summary>
public static class EnumerableExtensions
{
    public static bool IsNullable(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    /// <summary>
    /// Try to get the generic type of an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="type">The type to get the generic type.</param>
    /// <param name="underlyingType">The <c>T</c> of the <see cref="IEnumerable{T}"/>.</param>
    /// <returns>
    ///     Returns <c>true</c> if the type is an <see cref="IEnumerable{T}"/>, 
    ///     <c>false</c> otherwise.
    /// </returns>
    public static bool TryGetEnumerableGenericType(this Type type, [NotNullWhen(true)] out Type? underlyingType)
    {
        if (TryGetEnumerableGenericTypeCore(type, out underlyingType))
            return true;

        foreach (var i in type.GetInterfaces())
        {
            if (TryGetEnumerableGenericTypeCore(i, out underlyingType))
                return true;
        }

        underlyingType = null;
        return false;
    }

    private static bool TryGetEnumerableGenericTypeCore(Type type, [NotNullWhen(true)] out Type? underlyingType)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            underlyingType = type.GetGenericArguments()[0];
            return true;
        }

        underlyingType = null;
        return false;
    }
}
