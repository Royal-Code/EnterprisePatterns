
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoyalCode.Commands.AspNetCore.Infrastructure;
using RoyalCode.Commands.Handlers;
using RoyalCode.Commands.Abstractions;
using RoyalCode.Entities;

namespace RoyalCode.Commands.AspNetCore;

/// <summary>
/// <para>
///     Extension methods for creating routes for creation commands, implemented by default handlers.
/// </para>
/// </summary>
public static class CreationApi
{
    private static readonly Dictionary<CreatePathKey, CreatePathValue> creationPathMap = new();

#if NET7_0_OR_GREATER

    /// <summary>
    /// <para>
    ///     Maps a default command for creating an entity.
    /// </para>
    /// <para>
    ///     The default command uses the <see cref="CreateCommandHandler{TEntity, TModel}"/> to handle the
    ///     request and the <see cref="ICreationHandler{TModel, TEntity}"/> to create the entity.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TModel">The input type of the command.</typeparam>
    /// <param name="builder">The route builder.</param>
    /// <param name="pattern">The pattern of the route.</param>
    /// <param name="createdPath">The path to the created entity, optional.</param>
    /// <param name="formatPathWithValue">Determines if the path will be formatted with the value of the created entity.</param>
    /// <returns>The route handler builder.</returns>
    public static RouteHandlerBuilder MapDefaultCreate<TEntity, TModel>(this RouteGroupBuilder builder,
        string pattern = "/",string? createdPath = null, bool formatPathWithValue = true)
        where TEntity : class
        where TModel : class
    {
        var creationPathKey = new CreatePathKey { EntityType = typeof(TEntity), ModelType = typeof(TModel) };
        if (createdPath is null)
        {
            createdPath = "{0}";
            formatPathWithValue = true;
        }
        creationPathMap.Add(creationPathKey, new CreatePathValue { Path = createdPath, Format = formatPathWithValue });

        return builder.MapPost(pattern, CreationAsync<TEntity, TModel>);
    }

    /// <summary>
    /// <para>
    ///     Maps a default command for creating an entity.
    /// </para>
    /// <para>
    ///     The default command uses the <see cref="CreateCommandHandler{TEntity, TModel, TContext}"/> to handle the
    ///     request and the <see cref="ICreationHandler{TModel, TEntity, TContext}"/> to create the entity.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TModel">The input type of the command.</typeparam>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    /// <param name="builder">The route builder.</param>
    /// <param name="pattern">The pattern of the route.</param>
    /// <param name="createdPath">The path to the created entity, optional.</param>
    /// <param name="formatPathWithValue">Determines if the path will be formatted with the value of the created entity.</param>
    /// <returns>The route handler builder.</returns>
    public static RouteHandlerBuilder MapDefaultCreate<TEntity, TModel, TContext>(this RouteGroupBuilder builder,
        string pattern = "/", string? createdPath = null, bool formatPathWithValue = true)
        where TEntity : class
        where TModel : class
        where TContext : ICreationContext<TModel>
    {
        var creationPathKey = new CreatePathKey { EntityType = typeof(TEntity), ModelType = typeof(TModel) };
        if (createdPath is null)
        {
            createdPath = "{0}";
            formatPathWithValue = true;
        }
        creationPathMap.Add(creationPathKey, new CreatePathValue { Path = createdPath, Format = formatPathWithValue });

        return builder.MapPost(pattern, CreationAsync<TEntity, TModel, TContext>);
    }

    /// <summary>
    /// <para>
    ///     Maps a default command for creating an entity that is related to a root entity.
    /// </para>
    /// </summary>
    /// <typeparam name="TRootEntity">The type of the root entity.</typeparam>
    /// <typeparam name="TRootId">The type of the root entity identifier.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TModel">The input type of the command.</typeparam>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    /// <param name="builder">The route builder.</param>
    /// <param name="pattern">The pattern of the route.</param>
    /// <param name="createdPath">The path to the created entity, optional.</param>
    /// <param name="formatPathWithValue">Determines if the path will be formatted with the value of the created entity.</param>
    /// <returns>The route handler builder.</returns>
    public static RouteHandlerBuilder MapDefaultCreate<TRootEntity, TRootId, TEntity, TModel, TContext>(this RouteGroupBuilder builder,
        string pattern, string? createdPath = null, bool formatPathWithValue = true)
        where TRootEntity : class, IEntity<TRootId>
        where TEntity : class
        where TModel : class
        where TContext : ICreationContext<TModel, TRootEntity>
    {
        var creationPathKey = new CreatePathKey { EntityType = typeof(TEntity), ModelType = typeof(TModel) };
        if (createdPath is null)
        {
            createdPath = "{0}";
            formatPathWithValue = true;
        }
        creationPathMap.Add(creationPathKey, new CreatePathValue { Path = createdPath, Format = formatPathWithValue });

        return builder.MapPost(pattern, CreationAsync<TRootEntity, TRootId, TEntity, TModel, TContext>);
    }

#endif

    /// <summary>
    /// <para>
    ///     Maps a default command for creating an entity.
    /// </para>
    /// <para>
    ///     For .Net 7 or greater, use the <see cref="MapDefaultCreate{TEntity, TModel}(RouteGroupBuilder, string, string?, bool)"/> method.
    /// </para>
    /// <para>
    ///     The default command uses the <see cref="CreateCommandHandler{TEntity, TModel}"/> to handle the
    ///     request and the <see cref="ICreationHandler{TModel, TEntity}"/> to create the entity.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TModel">The input type of the command.</typeparam>
    /// <param name="app">The web application.</param>
    /// <param name="pattern">The pattern of the route.</param>
    /// <param name="createdPath">The path to the created entity, optional.</param>
    /// <param name="formatPathWithValue">Determines if the path will be formatted with the value of the created entity.</param>
    /// <returns>The route handler builder.</returns>
    public static RouteHandlerBuilder MapDefaultCreate<TEntity, TModel>(this WebApplication app,
        string pattern, string? createdPath = null, bool formatPathWithValue = true)
        where TEntity : class
        where TModel : class
    {
        var creationPathKey = new CreatePathKey { EntityType = typeof(TEntity), ModelType = typeof(TModel) };
        if (createdPath is null)
        {
            createdPath = $"{pattern}/{{0}}";
            formatPathWithValue = true;
        }
        creationPathMap.Add(creationPathKey, new CreatePathValue { Path = createdPath, Format = formatPathWithValue });

        return app.MapPost(pattern, CreationAsync<TEntity, TModel>);
    }

    /// <summary>
    /// <para>
    ///     Maps a default command for creating an entity.
    /// </para>
    /// <para>
    ///     For .Net 7 or greater, use the <see cref="MapDefaultCreate{TEntity, TModel}(RouteGroupBuilder, string, string?, bool)"/> method.
    /// </para>
    /// <para>
    ///     The default command uses the <see cref="CreateCommandHandler{TEntity, TModel, TContext}"/> to handle the
    ///     request and the <see cref="ICreationHandler{TModel, TEntity, TContext}"/> to create the entity.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TModel">The input type of the command.</typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="app">The web application.</param>
    /// <param name="pattern">The pattern of the route.</param>
    /// <param name="createdPath">The path to the created entity, optional.</param>
    /// <param name="formatPathWithValue">Determines if the path will be formatted with the value of the created entity.</param>
    /// <returns>The route handler builder.</returns>
    public static RouteHandlerBuilder MapDefaultCreate<TEntity, TModel, TContext>(this WebApplication app,
        string pattern, string? createdPath = null, bool formatPathWithValue = true)
        where TEntity : class
        where TModel : class
        where TContext : ICreationContext<TModel>
    {
        var creationPathKey = new CreatePathKey { EntityType = typeof(TEntity), ModelType = typeof(TModel) };
        if (createdPath is null)
        {
            createdPath = $"{pattern}/{{0}}";
            formatPathWithValue = true;
        }
        creationPathMap.Add(creationPathKey, new CreatePathValue { Path = createdPath, Format = formatPathWithValue });

        return app.MapPost(pattern, CreationAsync<TEntity, TModel, TContext>);
    }

    #region Request Handlers

    private static async Task<IResult> CreationAsync<TEntity, TModel>(
        TModel model,
        CreateCommandHandler<TEntity, TModel> handler,
        ILogger<CreateCommandHandler<TEntity, TModel>> logger,
        CancellationToken cancellationToken)
        where TEntity : class
        where TModel : class
    {
        try
        {
            var result = await handler.HandleAsync(model, cancellationToken);
            if (!result.Success)
                return result.ToResult();

            var creationPathKey = new CreatePathKey { EntityType = typeof(TEntity), ModelType = typeof(TModel) };
            var creationPath = creationPathMap[creationPathKey];

            return result.ToResult(creationPath.Path, creationPath.Format);
        }
        catch(Exception ex)
        {
            var problem = new ProblemDetails
            {
                Title = "Error on creation of entity.",
                Detail = string.Format("Error on creation of entity {0} with model {1}.", typeof(TEntity), typeof(TModel)),
                Status = StatusCodes.Status500InternalServerError
            };

            logger.LogError(ex, problem.Detail);

            return Results.Problem(problem);
        }
    }

    private static async Task<IResult> CreationAsync<TEntity, TModel, TContext>(
        TModel model,
        CreateCommandHandler<TEntity, TModel, TContext> handler,
        ILogger<CreateCommandHandler<TEntity, TModel>> logger,
        CancellationToken cancellationToken)
        where TEntity : class
        where TModel : class
        where TContext : ICreationContext<TModel>
    {
        try
        {
            var result = await handler.HandleAsync(model, cancellationToken);
            if (!result.Success)
                return result.ToResult();

            var creationPathKey = new CreatePathKey { EntityType = typeof(TEntity), ModelType = typeof(TModel) };
            var creationPath = creationPathMap[creationPathKey];

            return result.ToResult(creationPath.Path, creationPath.Format);
        }
        catch (Exception ex)
        {
            var problem = new ProblemDetails
            {
                Title = "Error on creation of entity.",
                Detail = string.Format("Error on creation of entity {0} with model {1}.", typeof(TEntity), typeof(TModel)),
                Status = StatusCodes.Status500InternalServerError
            };

            logger.LogError(ex, problem.Detail);

            return Results.Problem(problem);
        }
    }

    private static async Task<IResult> CreationAsync<TRootEntity, TRootId, TEntity, TModel, TContext>(
        TRootId id,
        TModel model,
        CreateCommandHandler<TRootEntity, TRootId, TEntity, TModel, TContext> handler,
        ILogger<CreateCommandHandler<TEntity, TModel>> logger,
        CancellationToken cancellationToken)
        where TRootEntity : class, IEntity<TRootId>
        where TEntity : class
        where TModel : class
        where TContext : ICreationContext<TModel, TRootEntity>
    {
        try
        {
            var result = await handler.HandleAsync(id, model, cancellationToken);
            if (!result.Success)
                return result.ToResult();

            var creationPathKey = new CreatePathKey { EntityType = typeof(TEntity), ModelType = typeof(TModel) };
            var creationPath = creationPathMap[creationPathKey];

            return result.ToResult(creationPath.Path, creationPath.Format);
        }
        catch (Exception ex)
        {
            var problem = new ProblemDetails
            {
                Title = "Error on creation of entity.",
                Detail = string.Format("Error on creation of entity {0} with model {1}.", typeof(TEntity), typeof(TModel)),
                Status = StatusCodes.Status500InternalServerError
            };

            logger.LogError(ex, problem.Detail);

            return Results.Problem(problem);
        }
    }

    #endregion
}
