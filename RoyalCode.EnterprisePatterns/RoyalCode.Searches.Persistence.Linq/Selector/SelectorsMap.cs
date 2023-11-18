﻿using RoyalCode.Searches.Persistence.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace RoyalCode.Searches.Persistence.Linq.Selector;

/// <summary>
/// A class that maps the types of the entity and Dto to the selector (<see cref="ISelector{TEntity, TDto}"/>).
/// </summary>
internal class SelectorsMap
{
    public static SelectorsMap Instance { get; } = new();

    private readonly Dictionary<(Type, Type), object> selectors = new();

    public object this[(Type, Type) key] => selectors[key];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey((Type, Type) key) => selectors.ContainsKey(key);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add((Type, Type) key, object value) => selectors.Add(key, value);

    public void Add<TEntity, TDto>(ISelector<TEntity, TDto> selector)
        where TEntity : class
        where TDto : class
    {
        var key = (typeof(TEntity), typeof(TDto));
        if (selectors.ContainsKey(key))
            throw new ArgumentException($"Selector for {key} already exists.");

        selectors.Add(key, selector);
    }

    public void Add<TEntity, TDto>(Expression<Func<TEntity, TDto>> selector)
        where TEntity : class
        where TDto : class
    {
        var key = (typeof(TEntity), typeof(TDto));
        if (selectors.ContainsKey(key))
            throw new ArgumentException($"Selector for {key} already exists.");

        selectors.Add(key, new InternalSelector<TEntity, TDto>(selector));
    }
}
