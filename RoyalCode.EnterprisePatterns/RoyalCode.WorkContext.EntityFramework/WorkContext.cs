using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.OperationHint.Abstractions;
using RoyalCode.Repositories.EntityFramework;
using RoyalCode.UnitOfWork.EntityFramework;
using RoyalCode.SmartSearch;
using RoyalCode.SmartSearch.Linq.Services;
using RoyalCode.SmartSearch.Linq.Sortings;
using RoyalCode.SmartSearch.Defaults;
using RoyalCode.WorkContext.EntityFramework.Internal;
using RoyalCode.SmartProblems;
using RoyalCode.Repositories;
using RoyalCode.WorkContext.Commands;
using RoyalCode.WorkContext.Querying;

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
    private readonly IHintPerformer? hintPerformer;

    /// <summary>
    /// Creates a new instance of <see cref="WorkContext{TDbContext}"/>.
    /// </summary>
    /// <param name="db">The database context.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="hintPerformer">The hint performer, optional.</param>
    public WorkContext(
        TDbContext db, 
        IServiceProvider? serviceProvider = null,
        IHintPerformer? hintPerformer = null)
        : base(db)
    {
        this.serviceProvider = serviceProvider
            ?? db.GetService<IDbContextOptions>()
                .Extensions.OfType<CoreOptionsExtension>().FirstOrDefault()
                ?.ApplicationServiceProvider
            ?? throw new InvalidOperationException("The service provider was not provided and it was not found in the database context");

        this.hintPerformer = hintPerformer;
    }

    /// <inheritdoc />
    public void AddHint<THint>(THint hint) where THint : Enum
    {
        var container = serviceProvider.GetService<IHintsContainer>();
        container?.AddHint(hint);
    }

    /// <inheritdoc />
    public ICriteria<TEntity> Criteria<TEntity>() where TEntity : class
    {
        var specifierFactory = serviceProvider.GetService<ISpecifierFactory>()
            ?? throw new InvalidOperationException($"The specifier factory was not configured for the work context");

        var orderByProvider = serviceProvider.GetService<IOrderByProvider>()
            ?? throw new InvalidOperationException($"The order by provider was not configured for the work context");

        var selectorFactory = serviceProvider.GetService<ISelectorFactory>()
            ?? throw new InvalidOperationException($"The selector factory was not configured for the work context");

        return new Criteria<TEntity>(
            new WorkContextCriteriaPerformer<TEntity>(Db, hintPerformer, specifierFactory, orderByProvider, selectorFactory));
    }

    /// <inheritdoc />
    public IRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        return new Repository<TDbContext, TEntity>(Db, hintPerformer);
    }

    /// <inheritdoc />
    public object GetService(Type serviceType) => serviceProvider.GetService(serviceType)
        ?? throw new InvalidOperationException($"The service of type {serviceType} was not found in the service provider of the work context");

    /// <inheritdoc />
    public TService GetService<TService>() => (TService)GetService(typeof(TService));

    /// <inheritdoc />
    public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(IQueryRequest<TEntity> request, CancellationToken ct = default)
        where TEntity : class
    {
        return QueryRequestHandler<TDbContext>.QueryAsync(request, Db, serviceProvider, ct);
    }

    /// <inheritdoc />
    public Task<IEnumerable<TModel>> QueryAsync<TEntity, TModel>(IQueryRequest<TEntity, TModel> request, CancellationToken ct = default)
         where TEntity : class
    {
        return QueryRequestHandler<TDbContext>.QueryAsync(request, Db, serviceProvider, ct);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<TEntity> QueryAsync<TEntity>(IAsyncQueryRequest<TEntity> request, CancellationToken ct = default)
         where TEntity : class
    {
        return QueryRequestHandler<TDbContext>.QueryAsync(request, Db, serviceProvider, ct);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<TModel> QueryAsync<TEntity, TModel>(IAsyncQueryRequest<TEntity, TModel> request, CancellationToken ct = default)
        where TEntity : class
    {
        return QueryRequestHandler<TDbContext>.QueryAsync(request, Db, serviceProvider, ct);
    }

    /// <inheritdoc />
    public Task<Result> SendAsync(ICommandRequest request, CancellationToken ct = default)
    {
        return CommandRequestHandler<TDbContext>.ExecuteAsync(request, this, serviceProvider, ct);
    }

    /// <inheritdoc />
    public Task<Result<TResponse>> SendAsync<TResponse>(ICommandRequest<TResponse> request, CancellationToken ct = default)
    {
        return CommandRequestHandler<TDbContext>.ExecuteAsync(request, this, serviceProvider, ct);
    }
}
