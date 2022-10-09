﻿
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Persistence.EntityFramework.UnitOfWork;
using RoyalCode.Repositories.Abstractions;

namespace RoyalCode.Persistence.EntityFramework.Repositories.Extensions;

/// <summary>
/// Extensions methods for <see cref="IUnitOfWorkBuilder{TDbContext}"/>
/// </summary>
public static class UnitOfWorkBuilderExtensions
{
    public static IUnitOfWorkBuilder<TDbContext> AddRepositories<TDbContext>(
        this IUnitOfWorkBuilder<TDbContext> builder, 
        Action<IRepositoryConfigurer<TDbContext>> configure)
        where TDbContext : DbContext
    {
        if (builder is null)
            throw new ArgumentNullException(nameof(builder));

        if (configure is null)
            throw new ArgumentNullException(nameof(configure));

        var configurer = new RepositoryConfigurer<TDbContext>(builder);
        configure(configurer);
        return builder;
    }
}

public interface IRepositoryConfigurer<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    /// Add a repository for an entity as a service, related to <see cref="DbContext"/> used by the unit of work.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <returns>The same instance.</returns>
    IRepositoryConfigurer<TDbContext> AddRepository<TEntity>()
        where TEntity : class;
}

internal class RepositoryConfigurer<TDbContext> : IRepositoryConfigurer<TDbContext>
    where TDbContext : DbContext
{
    private readonly IUnitOfWorkBuilder<TDbContext> builder;

    public RepositoryConfigurer(IUnitOfWorkBuilder<TDbContext> builder)
    {
        this.builder = builder;
    }

    /// <inheritdoc />
    public IRepositoryConfigurer<TDbContext> AddRepository<TEntity>() where TEntity : class
    {
        var repoType = typeof(IRepository<>).MakeGenericType(typeof(TEntity));

        builder.Services.Add(ServiceDescriptor.Describe(
            typeof(IRepository<>).MakeGenericType(typeof(TEntity)),
            typeof(Repository<,>).MakeGenericType(typeof(TDbContext), typeof(TEntity)),
            builder.Lifetime));

        foreach (var dataService in repoType.GetInterfaces())
        {
            builder.Services.Add(ServiceDescriptor.Describe(dataService, sp => sp.GetService(repoType)!, builder.Lifetime));
        }

        return this;
    }
}