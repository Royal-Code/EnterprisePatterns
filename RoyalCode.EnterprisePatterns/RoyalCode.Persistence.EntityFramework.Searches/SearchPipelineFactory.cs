using RoyalCode.Persistence.Searches.Abstractions.Pipeline;

namespace RoyalCode.Persistence.EntityFramework.Searches;

public class SearchPipelineFactory : ISearchPipelineFactory
{
    public ISearchPipeline<TEntity> Create<TEntity>() where TEntity : class
    {
        throw new NotImplementedException();
    }

    public ISearchPipeline<TDto> Create<TEntity, TDto>() where TEntity : class where TDto : class
    {
        throw new NotImplementedException();
    }
}