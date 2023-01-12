﻿using RoyalCode.Persistence.Searches.Abstractions.Linq.Sorter;

namespace RoyalCode.Persistence.EntityFramework.Searches.Infrastructure;

internal sealed class OrderByProvider : IOrderByProvider
{
    private readonly OrderByHandlersMap handlers;
    private readonly IOrderByGenerator? generator;

    public OrderByProvider(OrderByHandlersMap handlers, IOrderByGenerator? generator = null)
    {
        this.handlers = handlers;
        this.generator = generator;
    }

    public IOrderByHandler<TModel> GetDefaultHandler<TModel>() where TModel : class => GetHandler<TModel>("Id")!;

    public IOrderByHandler<TModel>? GetHandler<TModel>(string orderBy)
        where TModel : class
    {
        if (handlers.Contains((typeof(TModel), orderBy)))
            return (IOrderByHandler<TModel>)handlers[(typeof(TModel), orderBy)];

        var expression = generator?.Generate<TModel>(orderBy);
        if (expression is null)
            throw new InvalidOperationException($"The order by '{orderBy}' is not supported for the type '{typeof(TModel).Name}'.");

        var handler = OrderByHandler.Create<TModel>(expression);
        handlers.Add((typeof(TModel), orderBy), handler);
        return handler;
    }
}