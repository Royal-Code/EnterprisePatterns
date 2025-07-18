using Microsoft.EntityFrameworkCore;
using RoyalCode.Repositories.EntityFramework;
using RoyalCode.SmartSearch.EntityFramework.Services;
using RoyalCode.UnitOfWork.EntityFramework;
using RoyalCode.WorkContext.Abstractions;

namespace RoyalCode.WorkContext.EntityFramework;

#pragma warning disable S2326 // TDbContext

/// <summary>
/// A <see cref='IWorkContext'/> that is implemented with <see cref='DbContext'/> of type <typeparamref name='TDbContext'/>.
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public interface IWorkContext<TDbContext> : 
    IWorkContext, 
    IUnitOfWork<TDbContext>, 
    IEntityManager<TDbContext>,
    ISearchManager<TDbContext>
    where TDbContext : DbContext
{ }
