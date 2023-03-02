using Microsoft.EntityFrameworkCore;
using RoyalCode.Persistence.Searches.Abstractions.Base;
using RoyalCode.Persistence.Searches.Abstractions.Linq;
using RoyalCode.Persistence.Searches.Abstractions.Linq.Filter;
using RoyalCode.Persistence.Searches.Abstractions.Pipeline;

namespace RoyalCode.Persistence.EntityFramework.Searches;

/// <summary>
/// Default implementation of <see cref="IAllEntitiesPipeline{TEntity}"/>.
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
public sealed class AllEntitiesPipeline<TEntity> : SearchPipelineBase<TEntity>, IAllEntitiesPipeline<TEntity>
    where TEntity : class
{
    /// <inheritdoc />
    public AllEntitiesPipeline(
        IQueryableProvider<TEntity> queryableProvider,
        ISpecifierFactory specifierFactory, 
        ISorter<TEntity> sorter) 
        : base(queryableProvider, specifierFactory, sorter)
    { }

    /// <inheritdoc />
    public ICollection<TEntity> Execute(SearchCriteria criteria)
    {
        return PrepareQuery(criteria).ToList();
    }

    /// <inheritdoc />
    public async Task<ICollection<TEntity>> ExecuteAsync(
        SearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(criteria).ToListAsync(cancellationToken);
    }
}
