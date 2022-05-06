using RoyalCode.Persistence.Searches.Abstractions.Base;
using RoyalCode.Persistence.Searches.Abstractions.Linq;
using RoyalCode.Persistence.Searches.Abstractions.Linq.Filter;
using RoyalCode.Persistence.Searches.Abstractions.Pipeline;
using RoyalCode.Searches.Abstractions;

namespace RoyalCode.Persistence.EntityFramework.Searches;

public class SearchPipelineFactory
{
    
}

public class SearchPipeline<TEntity> : ISearchPipeline<TEntity>
    where TEntity : class
{
    private readonly IQueryableProvider<TEntity> queryableProvider;
    private readonly ISpecifierFactory specifierFactory;
    private readonly ISorter<TEntity> sorter;
    
    public IResultList<TEntity> Execute(SearchCriteria criteria)
    {
        var baseQuery = queryableProvider.GetQueryable();

        var queryFilters = criteria.Filters.Where(f => f.ModelType == typeof(TEntity)).ToList();
        if (queryFilters.Any())
        {
            var handler = new SpecifierHandler<TEntity>(specifierFactory, baseQuery);
            foreach (var searchFilter in queryFilters)
            {
                searchFilter.ApplyFilter(handler);
            }

            baseQuery = handler.Query;
        }

        var paginate = criteria.ItemsPerPage > 0;
        
        var sortedQuery = criteria.Sortings.Any()
            ? sorter.OrderBy(baseQuery, criteria.Sortings)
            : paginate
                ? sorter.DefaultOrderBy(baseQuery)
                : baseQuery;
        
        var executableQuery = paginate
            ? sortedQuery.Skip(criteria.ItemsPerPage * ((criteria.Page)))
        
        throw new NotImplementedException();
    }

    public Task<IResultList<TEntity>> ExecuteAsync(SearchCriteria criteria)
    {
        throw new NotImplementedException();
    }

    public Task<IAsyncResultList<TEntity>> AsyncExecuteAsync(SearchCriteria criteria)
    {
        throw new NotImplementedException();
    }
}