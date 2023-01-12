
using RoyalCode.Persistence.Searches.Abstractions.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace RoyalCode.Persistence.EntityFramework.Searches.Infrastructure;

/// <summary>
/// A class that maps the types of the entity and Dto to the selector (<see cref="ISelector{TEntity, TDto}"/>).
/// </summary>
internal class SelectorsMap
{
    public static SelectorsMap Instance { get; } = new();

    private readonly Dictionary<(Type, Type), object> selectors = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey((Type, Type) key) => selectors.ContainsKey(key);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add((Type, Type) key, object value) => selectors.Add(key, value);

    public object this[(Type, Type) key] => selectors[key];

    public void AddSelector<TEntity, TDto>(ISelector<TEntity, TDto> selector)
        where TEntity : class
        where TDto : class
    {
        var key = (typeof(TEntity), typeof(TDto));
        if (selectors.ContainsKey(key))
            throw new ArgumentException($"Selector for {key} already exists.");

        selectors.Add(key, selector);
    }

    public void AddSelector<TEntity, TDto>(Expression<Func<TEntity, TDto>> selector)
        where TEntity : class
        where TDto : class
    {
        var key = (typeof(TEntity), typeof(TDto));
        if (selectors.ContainsKey(key))
            throw new ArgumentException($"Selector for {key} already exists.");

        selectors.Add(key, new InternalSelector<TEntity, TDto>(selector));
    }
}
