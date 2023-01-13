using System.Linq.Expressions;

namespace RoyalCode.Persistence.EntityFramework.Searches.Configurations;

/// <summary>
///     Configure search components like specifier, order by and selector.
/// </summary>
public interface ISearchConfigurer
{
    /// <summary>
    /// Add a specifier function for the model and filter.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <param name="specifier">The specifier function that applies the filter over the model query.</param>
    /// <returns>The same instance of the configuration.</returns>
    ISearchConfigurer AddSpecifier<TModel, TFilter>(
        Func<IQueryable<TModel>, TFilter, IQueryable<TModel>> specifier); // adicionar ao Map

    /// <summary>
    /// Add a Order By expression for the model and property (<paramref name="orderBy"/>).
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="orderBy">The name of the property.</param>
    /// <param name="expression">The expression that represents the property.</param>
    /// <returns>The same instance of the configuration.</returns>
    ISearchConfigurer AddOrderBy<TModel, TProperty>(
        string orderBy, Expression<Func<TModel, TProperty>> expression)
        where TModel : class;

    /// <summary>
    /// Add a selector function expression for select values from the model to the DTO.
    /// </summary>
    /// <typeparam name="TEntity">The type of the model.</typeparam>
    /// <typeparam name="TDto">The type of the DTO.</typeparam>
    /// <param name="selector">The selector function.</param>
    /// <returns>The same instance of the configuration.</returns>
    ISearchConfigurer AddSelector<TEntity, TDto>(
        Expression<Func<TEntity, TDto>> selector)
        where TEntity : class
        where TDto : class;
}

