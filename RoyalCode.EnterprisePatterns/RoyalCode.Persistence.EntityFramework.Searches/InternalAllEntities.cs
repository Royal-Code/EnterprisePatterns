using Microsoft.EntityFrameworkCore;
using RoyalCode.Persistence.Searches.Abstractions.Pipeline;

namespace RoyalCode.Persistence.EntityFramework.Searches;

internal sealed class InternalAllEntities<TDbContext, TEntity> : AllEntities<TEntity>, IAllEntities<TDbContext, TEntity>
    where TEntity : class
    where TDbContext : DbContext
{
    public InternalAllEntities(IPipelineFactory factory) : base(factory) { }
}