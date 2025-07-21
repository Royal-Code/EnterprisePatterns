using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.WorkContext.Querying;
using System.Reflection;

namespace RoyalCode.WorkContext.EntityFramework.Querying.Configurations.Internals;

/// <summary>
/// Default configurer for query handlers in Entity Framework Core.
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
internal sealed class QueryConfigurer<TDbContext> : IQueryConfigurer<TDbContext>
    where TDbContext : DbContext
{
    public IServiceCollection Services { get; }

    public QueryConfigurer(IServiceCollection services)
    {
        Services = services;
    }

    public IQueryConfigurer<TDbContext> AddHandlersFromAssembly(Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        // foreach class in the assembly
        foreach (var type in assembly.GetTypes())
        {
            // if is a concrete class
            if (type.IsClass && !type.IsAbstract)
            {
                // check interfaces
                foreach (var iface in type.GetInterfaces())
                {
                    // check if implements:
                    // - IQueryHandler<TDbContext, TRequest, TEntity>
                    // - IQueryHandler<TDbContext, TRequest, TEntity, TModel>
                    // - IAsyncQueryHandler<TDbContext, TRequest, TEntity>
                    // - IAsyncQueryHandler<TDbContext, TRequest, TEntity, TModel>
                    if (iface.IsGenericType &&
                        (iface.GetGenericTypeDefinition() == typeof(IQueryHandler<,,>) ||
                            iface.GetGenericTypeDefinition() == typeof(IQueryHandler<,,,>) ||
                            iface.GetGenericTypeDefinition() == typeof(IAsyncQueryHandler<,,>) ||
                            iface.GetGenericTypeDefinition() == typeof(IAsyncQueryHandler<,,,>)))
                    {
                        // register the type
                        Services.Add(new ServiceDescriptor(iface, type, lifetime));
                    }
                }
            }
        }

        return this;
    }

    public IQueryConfigurer<TDbContext> AddHandler<THandler>(ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        ArgumentNullException.ThrowIfNull(Services);

        var type = typeof(THandler);
        var wasRegistered = false;

        // if is a concrete class
        if (type.IsClass && !type.IsAbstract)
        {
            // check interfaces
            foreach (var iface in type.GetInterfaces())
            {
                // check if implements:
                // - IQueryHandler<TDbContext, TRequest, TEntity>
                // - IQueryHandler<TDbContext, TRequest, TEntity, TModel>
                // - IAsyncQueryHandler<TDbContext, TRequest, TEntity>
                // - IAsyncQueryHandler<TDbContext, TRequest, TEntity, TModel>
                if (iface.IsGenericType &&
                    (iface.GetGenericTypeDefinition() == typeof(IQueryHandler<,,>) ||
                        iface.GetGenericTypeDefinition() == typeof(IQueryHandler<,,,>) ||
                        iface.GetGenericTypeDefinition() == typeof(IAsyncQueryHandler<,,>) ||
                        iface.GetGenericTypeDefinition() == typeof(IAsyncQueryHandler<,,,>)))
                {
                    // register the type
                    Services.Add(new ServiceDescriptor(iface, type, lifetime));
                    wasRegistered = true;
                }
            }
        }

        if (!wasRegistered)
        {
            throw new InvalidOperationException($"The type {type.FullName} does not implement any query handler interface.");
        }

        return this;
    }

    public IQueryConfigurer<TDbContext> Configure<TConfigurations>()
        where TConfigurations : class, IQueryHandlerConfigurations, new()
    {
        var configurations = new TConfigurations();
        var configurator = new QueryHandlerConfigurer<TDbContext>(Services);

        configurations.Configure(configurator);

        return this;
    }

    public IQueryConfigurer<TDbContext> Configure(IQueryHandlerConfigurations configurations)
    {
        ArgumentNullException.ThrowIfNull(configurations);

        var configurator = new QueryHandlerConfigurer<TDbContext>(Services);
        configurations.Configure(configurator);

        return this;
    }

    public IQueryConfigurer<TDbContext> Handle<TRequest, TEntity>(
        Func<TRequest, TDbContext, CancellationToken, Task<IEnumerable<TEntity>>> handler)
        where TRequest : IQueryRequest<TEntity>
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(handler);
        var queryHandler = new QueryHandler<TDbContext, TRequest, TEntity>(handler);
        Services.AddSingleton<IQueryHandler<TDbContext, TRequest, TEntity>>(queryHandler);
        return this;
    }

    public IQueryConfigurer<TDbContext> Handle<TRequest, TEntity, TModel>(
        Func<TRequest, TDbContext, CancellationToken, Task<IEnumerable<TModel>>> handler)
        where TRequest : IQueryRequest<TEntity, TModel>
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(handler);
        var queryHandler = new QueryHandler<TDbContext, TRequest, TEntity, TModel>(handler);
        Services.AddSingleton<IQueryHandler<TDbContext, TRequest, TEntity, TModel>>(queryHandler);
        return this;
    }

    public IQueryConfigurer<TDbContext> AsyncHandle<TRequest, TEntity>(
        Func<TRequest, TDbContext, CancellationToken, IAsyncEnumerable<TEntity>> handler)
        where TRequest : IAsyncQueryRequest<TEntity>
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(handler);
        var asyncQueryHandler = new AsyncQueryHandler<TDbContext, TRequest, TEntity>(handler);
        Services.AddSingleton<IAsyncQueryHandler<TDbContext, TRequest, TEntity>>(asyncQueryHandler);
        return this;
    }

    public IQueryConfigurer<TDbContext> AsyncHandle<TRequest, TEntity, TModel>(
        Func<TRequest, TDbContext, CancellationToken, IAsyncEnumerable<TModel>> handler)
        where TRequest : IAsyncQueryRequest<TEntity, TModel>
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(handler);
        var asyncQueryHandler = new AsyncQueryHandler<TDbContext, TRequest, TEntity, TModel>(handler);
        Services.AddSingleton<IAsyncQueryHandler<TDbContext, TRequest, TEntity, TModel>>(asyncQueryHandler);
        return this;
    }
}

