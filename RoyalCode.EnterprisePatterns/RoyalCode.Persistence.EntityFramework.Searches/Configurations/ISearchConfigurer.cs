using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace RoyalCode.Persistence.EntityFramework.Searches.Configurations;

/// <summary>
///     Configure searches for the unit of work.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
public interface ISearchConfigurer<out TDbContext>
    where TDbContext : DbContext
{

    ISearchConfigurer<TDbContext> AddSpecifier<TModel, TFilter>(
        Func<IQueryable<TModel>, TFilter, IQueryable<TModel>> specifier); // adicionar ao SpecifierFactory
}
