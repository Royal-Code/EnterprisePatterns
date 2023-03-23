
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RoyalCode.Commands.Abstractions.Defaults;

internal sealed class DefaultCreationHandler<TService, TEntity, TModel> : ICreationHandler<TEntity, TModel>
    where TEntity : class
    where TModel : class
{
    private readonly TService service;
    private readonly Func<TService, TModel, TEntity> createAction;

    public DefaultCreationHandler(TService service, Func<TService, TModel, TEntity> createAction)
    {
        this.service = service;
        this.createAction = createAction;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TEntity Create(TModel model) => createAction(service, model);
}

public static class CommandsErrorMessages
{

    public const string NotFoundpattern = "{0} {1} was not found for id: {2}";

    /// <summary>
    /// Utility to use extensions methods to set the error messages of commands in a fluent way.
    /// </summary>
    public static Lang Language => new();

    /// <summary>
    /// An internal class to use extension methods for the <see cref="CommandsErrorMessages"/> class.
    /// </summary>
    public class Lang { internal Lang() { } }
}

public static class GrammarGenre
{
    public static Func<Type, string> ProduceGrammarGenre { get; set; } = InternalProduceGrammarGenre;

    public static string Get<T>() => ProduceGrammarGenre(typeof(T));

    private static string InternalProduceGrammarGenre(Type type) => "The";
}

public static class DisplayNames
{
    private static readonly ConcurrentDictionary<Type, string> cache = new();

    public static Func<Type, string> ProduceDisplayName { get; set; } = InternalProduceDisplayName;

    public static string Get<T>() => ProduceDisplayName(typeof(T));

    private static string InternalProduceDisplayName(Type arg)
    {
        // check cache
        if (cache.TryGetValue(arg, out var displayName))
            return displayName;

        // get display name
        var displayAttribute = arg.GetCustomAttribute<DisplayNameAttribute>();
        if (displayAttribute is not null)
        {
            displayName = displayAttribute.DisplayName;
            cache.TryAdd(arg, displayName);
            return displayName;
        }

        // get name
        displayName = arg.Name;
        cache.TryAdd(arg, displayName);
        return displayName;
    }
}