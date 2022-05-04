using Microsoft.EntityFrameworkCore;
using RoyalCode.Persistence.Searches.Abstractions.Base;
using RoyalCode.Persistence.Searches.Abstractions.Linq;
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
    
    
    public IResultList<TEntity> Execute(SearchCriteria criteria)
    {
        var baseQuery = queryableProvider.GetQueryable();
        
        
        
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