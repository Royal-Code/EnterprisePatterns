using Microsoft.EntityFrameworkCore;
using RoyalCode.Searches.Persistence.Linq;

namespace RoyalCode.Searches.Persistence.EntityFramework;

internal sealed class QueryableProvider<TDbContext, TEntity> : IQueryableProvider<TEntity>
    where TDbContext : DbContext
    where TEntity : class
{
    private readonly TDbContext db;
    private readonly bool tracking;

    public QueryableProvider(TDbContext db, bool tracking = false)
    {
        this.db = db;
        this.tracking = tracking;
    }

    public IQueryable<TEntity> GetQueryable()
    {
        return tracking
            ? db.Set<TEntity>()
            : db.Set<TEntity>().AsNoTracking();
    }
}