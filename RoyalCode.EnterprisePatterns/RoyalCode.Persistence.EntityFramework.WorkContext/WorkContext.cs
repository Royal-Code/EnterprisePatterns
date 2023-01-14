﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Persistence.EntityFramework.Repositories;
using RoyalCode.Persistence.EntityFramework.UnitOfWork;
using RoyalCode.Repositories.Abstractions;
using RoyalCode.Searches.Abstractions;
using RoyalCode.WorkContext.Abstractions;

namespace RoyalCode.Persistence.EntityFramework.WorkContext;

/// <summary>
///     Default implementation of <see cref="IWorkContext"/> using Entity Framework and a <see cref="IServiceProvider"/>
///     to resolve the repositories and searches.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
public class WorkContext<TDbContext> : UnitOfWork<TDbContext>, IWorkContext
    where TDbContext : DbContext
{
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Creates a new instance of <see cref="WorkContext{TDbContext}"/>.
    /// </summary>
    /// <param name="db">The database context.</param>
    /// <param name="serviceProvider">The service provider.</param>
    public WorkContext(TDbContext db, IServiceProvider serviceProvider) : base(db)
    {
        this.serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public ISearch<TEntity> CreateSearch<TEntity>() where TEntity : class
    {
        var search = serviceProvider.GetService<Searches.ISearch<TDbContext, TEntity>>();
        if (search is null)
            throw new InvalidOperationException($"The search for the entity type {typeof(TEntity)} was not configured for the unit of work");

        return search;
    }

    /// <inheritdoc />
    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        var repository = serviceProvider.GetService<IRepository<TDbContext, TEntity>>();
        if (repository is null)
            throw new InvalidOperationException($"The repository for the entity type {typeof(TEntity)} was not configured for the unit of work");

        return repository;
    }
}