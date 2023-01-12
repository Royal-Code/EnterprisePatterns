using Microsoft.EntityFrameworkCore;
using RoyalCode.Persistence.Searches.Abstractions.Linq;

namespace RoyalCode.Persistence.EntityFramework.Searches.Infrastructure;

internal sealed class QueryableProvider<TDbContext, TEntity> : IQueryableProvider<TEntity>
    where TDbContext : DbContext
    where TEntity : class
{
    private readonly TDbContext db;

    public QueryableProvider(TDbContext db)
    {
        this.db = db;
    }

    public IQueryable<TEntity> GetQueryable()
    {
        return db.Set<TEntity>().AsNoTracking();
    }
}