﻿using RoyalCode.Extensions.PropertySelection;
using RoyalCode.Persistence.Searches.Abstractions.Linq.Selector.Converters;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace RoyalCode.Persistence.Searches.Abstractions.Linq.Selector;

/// <summary>
/// <para>
///     Default implementation of <see cref="ISelectorExpressionGenerator"/>.
/// </para>
/// <para>
///     This implementation can resolve selectors of plain types, 
///     where the DTO has properties with the same type of entity's properties.
/// </para>
/// </summary>
public sealed class DefaultSelectorExpressionGenerator : ISelectorExpressionGenerator, ISelectResolver
{
    private readonly IEnumerable<ISelectorPropertyResolver> propertyResolver
        = new List<ISelectorPropertyResolver>()
        {
            new NullableSelectorPropertyConverter(),
            new EnumSelectorPropertyConverter(),
            new SubSelectSelectorPropertyResolver()
        };

    /// <inheritdoc />
    public Expression<Func<TEntity, TDto>>? Generate<TEntity, TDto>()
        where TEntity : class
        where TDto : class
    {
        // get resolutions for the types.
        var resolutions = GetResolutions(typeof(TEntity), typeof(TDto), out var ctor);
        if (resolutions is null || ctor is null)
            return null;

        // create the input (entity) parameter.
        var parameter = Expression.Parameter(typeof(TEntity), "entity");

        // generate de bindings for create the new DTO
        var bindings = resolutions
            .Select(r => Expression.Bind(r.Match.OriginProperty, r.Converter.GetExpression(r.Match, parameter)));

        // create the new DTO with the bindings.
        var newDto = Expression.MemberInit(Expression.New(ctor), bindings);

        // generate the lambda expression
        return Expression.Lambda<Func<TEntity, TDto>>(newDto, parameter);
    }

    /// <inheritdoc />
    public IEnumerable<SelectResolution>? GetResolutions(Type entityType, Type dtoType, out ConstructorInfo? ctor)
    {
        // init
        ctor = null;

        // check if entity type and dto type are classes
        if (entityType.IsClass is false || dtoType.IsClass is false)
            return null;

        // get DTO constructor
        ctor = dtoType.GetConstructors().FirstOrDefault(c => c.GetParameters().Length == 0);
        if (ctor is null)
            return null;

        // create the match of the properties.
        var match = dtoType.MatchProperties(entityType);
        if (match is null) 
            return null;

        // for each property, check if was matched, check types, and create a resolution
        // if has an unmatch property, return null.
        var resolutions = new List<SelectResolution>();
        foreach (var item in match.PropertyMatches)
        {
            if (item.Match is false)
                return null;

            if (item.InvetedTypeMatch)
            {
                resolutions.Add(new SelectResolution(item, MemberSelectorPropertyConverter.Instance));
                continue;
            }

            if (TryGetConverter(item, out ISelectorPropertyConverter? converter))
                resolutions.Add(new SelectResolution(item, converter));
            else
                return null;
        }

        return resolutions;
    }

    private bool TryGetConverter(
        PropertyMatch propertyMatch,
        [NotNullWhen(true)] out ISelectorPropertyConverter? converter)
    {
        foreach (var r in propertyResolver)
        {
            if (r.CanConvert(propertyMatch, this, out var c))
            {
                converter = c!;
                return true;
            }
        }
        converter = null;
        return false;
    }

    private class MemberSelectorPropertyConverter : ISelectorPropertyConverter
    {
        public static readonly MemberSelectorPropertyConverter Instance = new();

        public Expression GetExpression(PropertyMatch selection, Expression parameter)
        {
            return selection.TargetSelection!.GetAccessExpression(parameter);
        }
    }
}
