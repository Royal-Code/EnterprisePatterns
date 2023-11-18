﻿namespace RoyalCode.Searches.Persistence.Abstractions.Extensions;

/// <summary>
/// Extension method to check if a type is nullable.
/// </summary>
public static class NullableExtensions
{
    /// <summary>
    /// Check if a type is nullable.
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns>True if it is nullable, false otherwise.</returns>
    public static bool IsNullableType(this Type type)
    {
        return type.GetNullabeUnderlyingType() is not null;
    }

    /// <summary>
    /// Check if a type is not nullable.
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns>True if it is not nullable, false otherwise.</returns>
    public static bool IsNotNullableType(this Type type)
    {
        return type.GetNullabeUnderlyingType() is null;
    }

    /// <summary>
    /// Try to get the underlying type of a nullable type.
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns>The underlying type if it is nullable, null otherwise.</returns>
    public static Type? GetNullabeUnderlyingType(this Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            return Nullable.GetUnderlyingType(type);

        return null;
    }
}