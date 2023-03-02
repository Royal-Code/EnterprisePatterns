using Microsoft.EntityFrameworkCore;
using RoyalCode.Persistence.Searches.Abstractions.Pipeline;

namespace RoyalCode.Persistence.EntityFramework.Searches;

internal sealed class InternalSearch<TDbContext, TEntity> : Search<TEntity>, ISearch<TDbContext, TEntity>
    where TEntity : class
    where TDbContext : DbContext
{
    public InternalSearch(IPipelineFactory factory) : base(factory) { }
}
