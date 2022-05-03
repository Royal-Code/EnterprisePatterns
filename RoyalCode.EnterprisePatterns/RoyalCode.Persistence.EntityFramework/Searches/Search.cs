using RoyalCode.Persistence.Searches.Abstractions.Base;
using RoyalCode.Searches.Abstractions;

namespace RoyalCode.Persistence.EntityFramework.Searches;

public class Search<TEntity> : SearchBase<TEntity>
    where TEntity : class
{
    
    
    
    public override IResultList<TEntity> ToList()
    {
        throw new NotImplementedException();
    }

    public override Task<IResultList<TEntity>> ToListAsync()
    {
        throw new NotImplementedException();
    }

    public override Task<IAsyncResultList<TEntity>> ToAsyncListAsync()
    {
        throw new NotImplementedException();
    }
}