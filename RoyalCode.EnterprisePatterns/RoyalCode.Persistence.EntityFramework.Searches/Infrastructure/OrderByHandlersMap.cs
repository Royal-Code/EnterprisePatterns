using System.Runtime.CompilerServices;

namespace RoyalCode.Persistence.EntityFramework.Searches.Infrastructure;

internal sealed class OrderByHandlersMap
{
    public static OrderByHandlersMap Instance { get; } = new OrderByHandlersMap();

    private readonly Dictionary<(Type, string), object> handlers = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains((Type, string) key) => handlers.ContainsKey(key);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add((Type modelType, string orderBy) key, object handler) => handlers.Add(key, handler);

    public object this[(Type, string) key] => handlers[key];
}
