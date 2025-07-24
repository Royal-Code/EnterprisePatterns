using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Repositories.EntityFramework;
using RoyalCode.SmartProblems.Entities;

namespace Microsoft.EntityFrameworkCore;

/// <summary>
/// Provides extension methods for querying DTOs from a <see cref="DbContext"/> using entity identifiers.
/// </summary>
public static class EFExtensions
{
    /// <summary>
    /// Finds a DTO mapped from an entity by its identifier.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TDto">The DTO type to project to.</typeparam>
    /// <param name="db">The <see cref="DbContext"/> instance.</param>
    /// <param name="id">The identifier of the entity.</param>
    /// <returns>The DTO instance if found; otherwise, <c>null</c>.</returns>
    public static TDto? Find<TEntity, TDto>(
        this DbContext db,
        object id)
        where TEntity : class
        where TDto : class
    {
        ArgumentNullException.ThrowIfNull(db);
        ArgumentNullException.ThrowIfNull(id);

        return SelectDtoById<TEntity, TDto>.FindByIdAndSelectDto(db, id).FirstOrDefault();
    }

    /// <summary>
    /// Asynchronously finds a DTO mapped from an entity by its identifier.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TDto">The DTO type to project to.</typeparam>
    /// <param name="db">The <see cref="DbContext"/> instance.</param>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="token">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the DTO instance if found; otherwise, <c>null</c>.</returns>
    public static async Task<TDto?> FindAsync<TEntity, TDto>(
        this DbContext db,
        object id,
        CancellationToken token = default)
        where TEntity : class
        where TDto : class
    {
        ArgumentNullException.ThrowIfNull(db);
        ArgumentNullException.ThrowIfNull(id);

        return await SelectDtoById<TEntity, TDto>.FindByIdAndSelectDto(db, id).FirstOrDefaultAsync(token);
    }

    /// <summary>
    /// Asynchronously tries to find a DTO mapped from an entity by its identifier, returning an entry with the DTO and identifier.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TDto">The DTO type to project to.</typeparam>
    /// <typeparam name="TId">The type of the identifier.</typeparam>
    /// <param name="db">The <see cref="DbContext"/> instance.</param>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="token">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="FindResult{TDto, TId}"/> with the DTO and identifier.</returns>
    public static async Task<FindResult<TDto, TId>> TryFindAsync<TEntity, TDto, TId>(
        this DbContext db,
        Id<TEntity, TId> id,
        CancellationToken token = default)
        where TEntity : class
        where TDto : class
    {
        ArgumentNullException.ThrowIfNull(db);

        var dto = await SelectDtoById<TEntity, TDto>.FindByIdAndSelectDto(db, id.Value!).FirstOrDefaultAsync(token);

        return new FindResult<TDto, TId>(dto, id.Value);
    }

    /// <summary>
    /// Gets a service registered in the application's service provider associated with the <see cref="DbContext"/>.
    /// </summary>
    /// <typeparam name="TService">The type of service to obtain.</typeparam>
    /// <param name="accessor">Access to the service provider of the DbContext.</param>
    /// <returns>The requested service instance.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If there is no service of type <typeparamref name="TService"/> registered
    ///     in the application's service provider associated with the <see cref="DbContext"/>,
    ///     or when the application's service provider is not configured for the <see cref="DbContext"/>.
    /// </exception>
    public static TService GetApplicationService<TService>(this IInfrastructure<IServiceProvider> accessor)
        where TService : class
    {
        var sp = accessor.Instance;

        return (sp.GetService<IDbContextOptions>()
                ?.Extensions.OfType<CoreOptionsExtension>().FirstOrDefault()
                ?.ApplicationServiceProvider
                ?.GetRequiredService<TService>())
                ?? throw new InvalidOperationException(
                    $"No service of type '{typeof(TService).FullName}' is registered in the DbContext.");
    }
}
