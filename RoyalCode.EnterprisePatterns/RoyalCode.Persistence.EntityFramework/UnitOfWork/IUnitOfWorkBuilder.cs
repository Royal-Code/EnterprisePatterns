using Microsoft.EntityFrameworkCore;

namespace RoyalCode.Persistence.EntityFramework.UnitOfWork;

/// <summary>
/// <para>
///     Interface to configure one unit of work with the DbContext.
/// </para>
/// <para>
///     It is designed to work with dependecy injection.
/// </para>
/// </summary>
/// <typeparam name="TDbContext">The type of DbContext for the unit of work.</typeparam>
public interface IUnitOfWorkBuilder<TDbContext>
    where TDbContext : DbContext
{
    IUnitOfWorkBuilder<TDbContext> ConfigureDbContextPool(Action<DbContextOptionsBuilder> configurer);
    
    IUnitOfWorkBuilder<TDbContext> ConfigureDbContextPool(Action<IServiceProvider, DbContextOptionsBuilder> configurer);
    
    IUnitOfWorkBuilder<TDbContext> ConfigureDbContext(Action<DbContextOptionsBuilder> configurer);
    
    IUnitOfWorkBuilder<TDbContext> ConfigureDbContext(Action<IServiceProvider, DbContextOptionsBuilder> configurer);
    
    IUnitOfWorkBuilder<TDbContext> AddRepository<TEntity>()
        where TEntity : class;
}