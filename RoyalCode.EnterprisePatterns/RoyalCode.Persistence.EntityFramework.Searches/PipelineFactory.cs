using Microsoft.EntityFrameworkCore;
using RoyalCode.Persistence.Searches.Abstractions.Linq.Filter;
using RoyalCode.Persistence.Searches.Abstractions.Linq.Selector;
using RoyalCode.Persistence.Searches.Abstractions.Linq.Sorter;
using RoyalCode.Persistence.Searches.Abstractions.Pipeline;

namespace RoyalCode.Persistence.EntityFramework.Searches;

/// <inheritdoc />
public sealed class PipelineFactory<TDbContext> : IPipelineFactory
    where TDbContext : DbContext
{
    private readonly TDbContext db;
    private readonly ISpecifierFactory specifierFactory;
    private readonly IOrderByProvider orderByProvider;
    private readonly ISelectorFactory selectorFactory;

    /// <summary>
    /// <para>
    ///     Create a new instance of <see cref="PipelineFactory{TDbContext}"/>.
    /// </para>
    /// </summary>
    /// <param name="db">The database context.</param>
    /// <param name="specifierFactory">The specifier factory.</param>
    /// <param name="orderByProvider">The order by provider.</param>
    /// <param name="selectorFactory">The selector factory.</param>
    public PipelineFactory(
        TDbContext db,
        ISpecifierFactory specifierFactory,
        IOrderByProvider orderByProvider,
        ISelectorFactory selectorFactory)
    {
        this.db = db;
        this.specifierFactory = specifierFactory;
        this.orderByProvider = orderByProvider;
        this.selectorFactory = selectorFactory;
    }

    /// <inheritdoc />
    public ISearchPipeline<TEntity> Create<TEntity>() where TEntity : class
    {
        var queryableProvider = new QueryableProvider<TDbContext, TEntity>(db);
        var sorter = new DefaultSorter<TEntity>(orderByProvider);
        return new SearchPipeline<TEntity>(queryableProvider, specifierFactory, sorter);
    }

    /// <inheritdoc />
    public ISearchPipeline<TDto> Create<TEntity, TDto>() where TEntity : class where TDto : class
    {
        var queryableProvider = new QueryableProvider<TDbContext, TEntity>(db);
        var sorter = new DefaultSorter<TEntity>(orderByProvider);
        var selector = selectorFactory.Create<TEntity, TDto>();
        return new SearchPipeline<TEntity, TDto>(queryableProvider, specifierFactory, sorter, selector);
    }

    /// <inheritdoc />
    public IAllEntitiesPipeline<TEntity> CreateAllEntities<TEntity>() where TEntity : class
    {
        var queryableProvider = new QueryableProvider<TDbContext, TEntity>(db, true);
        var sorter = new DefaultSorter<TEntity>(orderByProvider);
        return new AllEntitiesPipeline<TEntity>(queryableProvider, specifierFactory, sorter);
    }
}