using Microsoft.EntityFrameworkCore;
using RoyalCode.OperationHint.Abstractions;

namespace RoyalCode.Persistence.EntityFramework.Repositories;

internal sealed class InternalRepository<TDbContext, TEntity> : Repository<TDbContext, TEntity>
    where TEntity : class
    where TDbContext : DbContext
{
    public InternalRepository(TDbContext dbContext, IHintPerformer? hintPerformer = null) 
        : base(dbContext, hintPerformer) 
    { }
}
