using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.OperationHint.Abstractions;
using RoyalCode.Repositories.EntityFramework;
using RoyalCode.UnitOfWork.EntityFramework;
using RoyalCode.Repositories.Abstractions;
using RoyalCode.WorkContext.Abstractions;
using RoyalCode.SmartSearch;
using RoyalCode.SmartSearch.Linq.Services;
using RoyalCode.SmartSearch.Linq.Sortings;
using RoyalCode.SmartSearch.Defaults;
using RoyalCode.WorkContext.Abstractions.Quering;
using RoyalCode.WorkContext.EntityFramework.Internal;

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
    public ICriteria<TEntity> Criteria<TEntity>() where TEntity : class
    {
        var specifierFactory = serviceProvider.GetService<ISpecifierFactory>() 
            ?? throw new InvalidOperationException($"The specifier factory was not configured for the work context");

        var orderByProvider = serviceProvider.GetService<IOrderByProvider>() 
            ?? throw new InvalidOperationException($"The order by provider was not configured for the work context");
        
        var selectorFactory = serviceProvider.GetService<ISelectorFactory>()
            ?? throw new InvalidOperationException($"The selector factory was not configured for the work context");

        var hintPerformer = serviceProvider.GetService<IHintPerformer>();

        return new Criteria<TEntity>(
            new WorkContextCriteriaPerformer<TEntity>(Db, hintPerformer, specifierFactory, orderByProvider, selectorFactory));
    }

    /// <inheritdoc />
    public IRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        var hintPerformer = serviceProvider.GetService<IHintPerformer>();
        return new Repository<TDbContext, TEntity>(Db, hintPerformer);
    }

    /// <inheritdoc />
    public object GetService(Type serviceType) => serviceProvider.GetService(serviceType)
        ?? throw new InvalidOperationException($"The service of type {serviceType} was not found in the service provider of the work context");
    
    /// <inheritdoc />
    public TService GetService<TService>() => (TService)GetService(typeof(TService));

    /// <inheritdoc />
    public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(IQueryRequest<TEntity> request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<IEnumerable<TModel>> QueryAsync<TEntity, TModel>(IQueryRequest<TEntity, TModel> request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IAsyncEnumerable<TEntity> QueryAsync<TEntity>(IAsyncQueryRequest<TEntity> request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IAsyncEnumerable<TModel> QueryAsync<TEntity, TModel>(IAsyncQueryRequest<TEntity, TModel> request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
