using RoyalCode.Commands.Abstractions;
using RoyalCode.Entities;
using RoyalCode.OperationResults;
using RoyalCode.WorkContext.Abstractions;

namespace RoyalCode.Commands.Handlers;

/// <summary>
/// A default handler for entity creation commands.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TModel">The input type of the command.</typeparam>
public sealed class CreateCommandHandler<TEntity, TModel>
    where TEntity : class
    where TModel : class
{
    private readonly IWorkContext context;
    private readonly IEnumerable<IValidator<TModel>> validators;
    private readonly ICreationHandler<TModel, TEntity> creationHandler;

    /// <summary>
    /// Creates a new instance of <see cref="CreateCommandHandler{TEntity, TModel}"/>.
    /// </summary>
    /// <param name="context">The work context.</param>
    /// <param name="validators">The validators for the model.</param>
    /// <param name="creationHandler">The creation handler to create the entity.</param>
    public CreateCommandHandler(
        IWorkContext context,
        IEnumerable<IValidator<TModel>> validators,
        ICreationHandler<TModel, TEntity> creationHandler)
    {
        this.context = context;
        this.validators = validators;
        this.creationHandler = creationHandler;
    }

    /// <summary>
    /// <para>
    ///     Handle the command.
    /// </para>
    /// <para>
    ///     Apply the validators, create the entity and save the changes.
    /// </para>
    /// </summary>
    /// <param name="model">The input model.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The result of the operation with the created entity.</returns>
    public async Task<IOperationResult<TEntity>> HandleAsync(TModel model, CancellationToken token) // TODO: Usar OperationResult
    {
        foreach (var validator in validators)
        {
            var result = validator.Validate(model);
            if (result.Failure)
                return result.ToValue<TEntity>();
        }

        if (creationHandler is IValidationHandler<TModel> validationHandler)
        {
            var result = validationHandler.Validate(context, model);
            if (!result.Success)
                return result.ToValue<TEntity>();
        }

        var entity = creationHandler.Create(model);
        context.GetRepository<TEntity>().Add(entity);

        var saveResult = await context.SaveAsync(token);
        return saveResult.ToValue(entity);
    }
}

/// <summary>
/// A default handler for entity creation commands.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TModel">The input type of the command.</typeparam>
/// <typeparam name="TContext">The type of the context used to store other loaded data to be used in the creation of the entity.</typeparam>
public sealed class CreateCommandHandler<TEntity, TModel, TContext>
    where TEntity : class
    where TModel : class
    where TContext : ICreationContext<TModel>
{
    private readonly IWorkContext context;
    private readonly IEnumerable<IValidator<TModel>> validators;
    private readonly ICreationHandler<TContext, TModel, TEntity> creationHandler;

    /// <summary>
    /// Creates a new instance of <see cref="CreateCommandHandler{TEntity, TModel, TContext}"/>.
    /// </summary>
    /// <param name="context">The work context.</param>
    /// <param name="validators">The validators for the model.</param>
    /// <param name="creationHandler">The creation handler to create the entity.</param>
    public CreateCommandHandler(
        IWorkContext context,
        IEnumerable<IValidator<TModel>> validators,
        ICreationHandler<TContext, TModel, TEntity> creationHandler)
    {
        this.context = context;
        this.validators = validators;
        this.creationHandler = creationHandler;
    }

    /// <summary>
    /// <para>
    ///     Handle the command.
    /// </para>
    /// <para>
    ///     Apply the validators, create the context, create the entity and save the changes.
    /// </para>
    /// </summary>
    /// <param name="model">The input model.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The result of the operation with the created entity.</returns>
    public async Task<IOperationResult<TEntity>> HandleAsync(TModel model, CancellationToken token)
    {
        foreach (var validator in validators)
        {
            var result = validator.Validate(model);
            if (result.Failure)
                return result.ToValue<TEntity>();
        }

        if (creationHandler is IValidationHandler<TModel> validationHandler)
        {
            var result = validationHandler.Validate(context, model);
            if (!result.Success)
                return result.ToValue<TEntity>();
        }

        var creationContextResult = await creationHandler.CreateContextAsync(context, model, token);
        if (!creationContextResult.Success)
            return creationContextResult.ToValue<TEntity>();

        var creationContext = creationContextResult.Value!;
        if (creationContext is IValidableContext validable)
        {
            var result = validable.Validate();
            if (!result.Success)
                return result.ToValue<TEntity>();
        }

        var entity = creationHandler.Create(creationContextResult.Value!);
        context.GetRepository<TEntity>().Add(entity);

        var saveResult = await context.SaveAsync(token);
        return saveResult.ToValue(entity);
    }
}

/// <summary>
/// A default handler for entity creation commands.
/// </summary>
/// <typeparam name="TRootEntity">The aggregate root entity type.</typeparam>
/// <typeparam name="TRootId">The aggregate root entity id type.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TModel">The input type of the command.</typeparam>
/// <typeparam name="TContext">The type of the context used to store other loaded data to be used in the creation of the entity.</typeparam>
public sealed class CreateCommandHandler<TRootEntity, TRootId, TEntity, TModel, TContext>
    where TRootEntity : class, IEntity<TRootId>
    where TEntity : class
    where TModel : class
    where TContext : ICreationContext<TModel, TRootEntity>
{
    private readonly IWorkContext context;
    private readonly IEnumerable<IValidator<TModel>> validators;
    private readonly ICreationHandler<TContext, TModel, TRootEntity, TEntity> creationHandler;

    /// <summary>
    /// Creates a new instance of <see cref="CreateCommandHandler{TRootEntity, TRootId, TEntity, TModel, TContext}"/>.
    /// </summary>
    /// <param name="context">The work context.</param>
    /// <param name="validators">The validators for the model.</param>
    /// <param name="creationHandler">The creation handler to create the entity.</param>
    public CreateCommandHandler(
        IWorkContext context,
        IEnumerable<IValidator<TModel>> validators,
        ICreationHandler<TContext, TModel, TRootEntity, TEntity> creationHandler)
    {
        this.context = context;
        this.validators = validators;
        this.creationHandler = creationHandler;
    }

    /// <summary>
    /// <para>
    ///     Handle the command.
    /// </para>
    /// <para>
    ///     Apply the validators, find the aggregate root entity,
    ///     create the context, create the entity and save the changes.
    /// </para>
    /// </summary>
    /// <param name="id">The id of the aggregate root entity.</param>
    /// <param name="model">The input model.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The result of the operation with the created entity.</returns>
    public async Task<IOperationResult<TEntity>> HandleAsync(TRootId id, TModel model, CancellationToken token)
    {
        foreach (var validator in validators)
        {
            var result = validator.Validate(model);
            if (result.Failure)
                return result.ToValue<TEntity>();
        }

        if (creationHandler is IValidationHandler<TModel> validationHandler)
        {
            var result = validationHandler.Validate(context, model);
            if (!result.Success)
                return result.ToValue<TEntity>();
        }

        var rootEntity = await context.GetRepository<TRootEntity>().FindAsync(id!);
        if (rootEntity is null)
            return ValueResult.NotFound<TEntity>(CommandsErrorMessages.CreateNotFoundMessage<TRootEntity>(id), nameof(id));

        var creationContextResult = await creationHandler.CreateContextAsync(context, model, rootEntity, token);
        if (!creationContextResult.Success)
            return creationContextResult.ToValue<TEntity>();

        var creationContext = creationContextResult.Value!;
        if (creationContext is IValidableContext validable)
        {
            var result = validable.Validate();
            if (!result.Success)
                return result.ToValue<TEntity>();
        }

        var entity = creationHandler.Create(creationContextResult.Value!);
        context.GetRepository<TEntity>().Add(entity);

        var saveResult = await context.SaveAsync(token);
        return saveResult.ToValue(entity);
    }
}
