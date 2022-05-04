namespace RoyalCode.Persistence.Searches.Abstractions.Pipeline;

/// <summary>
/// <para>
///     Factory to create search pipelines (<see cref="ISearchPipeline{TModel}"/>).
/// </para>
/// </summary>
public interface ISearchPipelineFactory
{
    /// <summary>
    /// Creates a <see cref="ISearchPipeline{TModel}"/> to query entities.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <returns>A new instance of a pipeline to execute the search.</returns>
    ISearchPipeline<TEntity> Create<TEntity>()
        where TEntity : class;

    /// <summary>
    /// Creates a new <see cref="ISearchPipeline{TModel}"/> to select DTOs from the query of entities.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TDto">The DTO (to be selected) type.</typeparam>
    /// <returns>A new instance of a pipeline to execute the search.</returns>
    ISearchPipeline<TDto> Create<TEntity, TDto>()
        where TEntity : class
        where TDto : class;
}