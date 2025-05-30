﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.OperationHint.Abstractions;
using RoyalCode.Repositories.EntityFramework;
using RoyalCode.UnitOfWork.EntityFramework;
using RoyalCode.Repositories.Abstractions;
using RoyalCode.SmartSearch.Abstractions;
using RoyalCode.WorkContext.Abstractions;

namespace RoyalCode.WorkContext.EntityFramework;

/// <summary>
///     Default implementation of <see cref="IWorkContext"/> using Entity Framework and a <see cref="IServiceProvider"/>
///     to resolve the repositories and searches.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
public class WorkContext<TDbContext> : UnitOfWork<TDbContext>, IWorkContext<TDbContext>
    where TDbContext : DbContext
{
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Creates a new instance of <see cref="WorkContext{TDbContext}"/>.
    /// </summary>
    /// <param name="db">The database context.</param>
    /// <param name="serviceProvider">The service provider.</param>
    public WorkContext(TDbContext db, IServiceProvider? serviceProvider = null) : base(db)
    {
        this.serviceProvider = serviceProvider
            ?? db.GetService<IDbContextOptions>()
                .Extensions.OfType<CoreOptionsExtension>().FirstOrDefault()
                ?.ApplicationServiceProvider
            ?? throw new InvalidOperationException("The service provider was not provided and it was not found in the database context");
    }

    /// <inheritdoc />
    public void AddHint<THint>(THint hint) where THint : Enum
    {
        var container = serviceProvider.GetService<IHintsContainer>();
        container?.AddHint(hint);
    }

    /// <inheritdoc />
    public IAllEntities<TEntity> All<TEntity>() where TEntity : class
    {
        var search = serviceProvider.GetService<SmartSearch.EntityFramework.Internals.IAllEntities<TDbContext, TEntity>>();
        return search is null
            ? throw new InvalidOperationException($"The search for all the entities of type {typeof(TEntity)} was not configured for the work context")
            : (IAllEntities<TEntity>)search;
    }

    /// <inheritdoc />
    public ISearch<TEntity> Search<TEntity>() where TEntity : class
    {
        var search = serviceProvider.GetService<SmartSearch.EntityFramework.Internals.ISearch<TDbContext, TEntity>>();
        return search is null
            ? throw new InvalidOperationException($"The search for the entity type {typeof(TEntity)} was not configured for the work context")
            : (ISearch<TEntity>)search;
    }

    /// <inheritdoc />
    public IRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        var repository = serviceProvider.GetService<IRepository<TDbContext, TEntity>>();
        return repository is null
            ? throw new InvalidOperationException($"The repository for the entity type {typeof(TEntity)} was not configured for the work context")
            : (IRepository<TEntity>)repository;
    }

    /// <inheritdoc />
    public object GetService(Type serviceType) => serviceProvider.GetService(serviceType)
        ?? throw new InvalidOperationException($"The service of type {serviceType} was not found in the service provider of the work context");
    
    /// <inheritdoc />
    public TService GetService<TService>() => (TService)GetService(typeof(TService));
}
