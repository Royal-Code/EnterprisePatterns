﻿namespace RoyalCode.Searches.Abstractions;

/// <summary>
/// <para>
///     Attribute that indicates that a property is a criterion.
/// </para>
/// <para>
///     This can be use in property of filters to automatic generate a filter function.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class CriterionAttribute : Attribute
{
    /// <summary>
    /// <para>
    ///     The operator used to filter.
    /// </para>
    /// </summary>
    public CriterionOperator Operator { get; set; }

    /// <summary>
    /// <para>
    ///     The name of the target property.
    /// </para>
    /// <para>
    ///     Optional, when not informed then the property name will be used.
    /// </para>
    /// </summary>
    public string? TargetProperty { get; set; }

    /// <summary>
    /// <para>
    ///     Ignore the property. Do not use to apply filters.
    /// </para>
    /// </summary>
    public bool Ignore { get; set; }
}
