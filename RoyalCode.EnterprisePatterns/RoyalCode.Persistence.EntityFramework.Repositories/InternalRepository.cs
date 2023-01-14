using Microsoft.EntityFrameworkCore;

namespace RoyalCode.Persistence.EntityFramework.Repositories;

internal sealed class InternalRepository<TDbContext, TEntity> : Repository<TDbContext, TEntity>
    where TEntity : class
    where TDbContext : DbContext
{
    public InternalRepository(TDbContext dbContext) : base(dbContext) { }
}
