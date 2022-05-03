using System.Linq.Expressions;
using RoyalCode.Searches.Abstractions;

namespace RoyalCode.Persistence.Searches.Abstractions.Base;

public abstract class SearchBase<TEntity> : ISearch<TEntity>
    where TEntity : class
{
    protected readonly SearchCriteria criteria;
    
    public ISearch<TEntity> UsePages(int itemsPerPage = 10)
    {
        criteria.ItemsPerPage = itemsPerPage;
        return this;
    }

    public ISearch<TEntity> FetchPage(int pageNumber)
    {
        criteria.Page = pageNumber;
        return this;
    }

    public ISearch<TEntity> UseLastCount(int lastCount)
    {
        criteria.LastCount = lastCount;
        return this;
    }

    public ISearch<TEntity> UseCount(bool useCount = true)
    {
        criteria.UseCount = useCount;
        return this;
    }

    public ISearch<TEntity> FilterBy<TFilter>(TFilter filter) 
        where TFilter : class
    {
        criteria.AddFilter(typeof(TEntity), typeof(TFilter), filter);
        return this;
    }

    public ISearch<TEntity> OrderBy(ISorting sorting)
    {
        criteria.AddSorting(sorting);
        return this;
    }

    public ISearch<TEntity, TDto> Select<TDto>() 
        where TDto : class
    {
        throw new NotImplementedException();
    }

    public ISearch<TEntity, TDto> Select<TDto>(Expression<Func<TEntity, TDto>> selectExpression) 
        where TDto : class
    {
        throw new NotImplementedException();
    }

    public abstract IResultList<TEntity> ToList();

    public abstract Task<IResultList<TEntity>> ToListAsync();

    public abstract Task<IAsyncResultList<TEntity>> ToAsyncListAsync();
}

public abstract class SearchBase<TEntity, TDto> : ISearch<TEntity, TDto>
    where TEntity : class
    where TDto : class
{
    protected readonly SearchCriteria criteria;
    
    public ISearch<TEntity, TDto> UsePages(int itemsPerPage = 10)
    {
        criteria.ItemsPerPage = itemsPerPage;
        return this;
    }

    public ISearch<TEntity, TDto> FetchPage(int pageNumber)
    {
        criteria.Page = pageNumber;
        return this;
    }

    public ISearch<TEntity, TDto> UseLastCount(int lastCount)
    {
        criteria.LastCount = lastCount;
        return this;
    }

    public ISearch<TEntity, TDto> UseCount(bool useCount = true)
    {
        criteria.UseCount = useCount;
        return this;
    }
    
    public ISearch<TEntity, TDto> FilterBy<TFilter>(TFilter filter) 
        where TFilter : class
    {
        criteria.AddFilter(typeof(TDto), typeof(TFilter), filter);
        return this;
    }

    public abstract IResultList<TDto> ToList();

    public abstract Task<IResultList<TDto>> ToListAsync();

    public abstract Task<IAsyncResultList<TDto>> ToAsyncListAsync();
}