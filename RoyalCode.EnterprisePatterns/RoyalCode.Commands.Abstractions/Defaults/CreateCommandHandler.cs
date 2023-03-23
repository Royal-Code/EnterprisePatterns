
using RoyalCode.OperationResult;
using RoyalCode.WorkContext.Abstractions;

namespace RoyalCode.Commands.Abstractions.Defaults;

public sealed class CreateCommandHandler<TEntity, TModel> : ICreateCommandHandler<TEntity, TModel>
    where TEntity : class
    where TModel : class
{
    private readonly IWorkContext context;
    private readonly IEnumerable<IValidator<TModel>> validators;
    private readonly ICreationHandler<TEntity, TModel> creationHandler;

    public CreateCommandHandler(
        IWorkContext context,
        IEnumerable<IValidator<TModel>> validators,
        ICreationHandler<TEntity, TModel> creationHandler)
    {
        this.context = context;
        this.validators = validators;
        this.creationHandler = creationHandler;
    }

    public async Task<IOperationResult<TEntity>> HandleAsync(TModel model, CancellationToken token)
    {
        foreach (var validator in validators)
        {
            var result = validator.Validate(model);
            if (result.Failure)
                return result.ToValue<TEntity>();
        }

        var entity = creationHandler.Create(model);

        context.GetRepository<TEntity>().Add(entity);

        var saveResult = await context.SaveAsync(token);
        return saveResult.ToValue(entity);
    }
}

public sealed class CreateCommandHandler<TEntity, TContext, TModel> : ICreateCommandHandler<TEntity, TModel>
    where TEntity : class
    where TModel : class
    where TContext : ICommandContext<TModel>
{
    private readonly IWorkContext context;
    private readonly IEnumerable<IValidator<TModel>> validators;
    private readonly ICommandContextFactory commandContextFactory;
    private readonly ICreationHandler<TEntity, TContext, TModel> creationHandler;

    public CreateCommandHandler(
        IWorkContext context,
        IEnumerable<IValidator<TModel>> validators,
        ICommandContextFactory commandContextFactory,
        ICreationHandler<TEntity, TContext, TModel> creationHandler)
    {
        this.context = context;
        this.validators = validators;
        this.commandContextFactory = commandContextFactory;
        this.creationHandler = creationHandler;
    }

    public async Task<IOperationResult<TEntity>> HandleAsync(TModel model, CancellationToken token)
    {
        foreach (var validator in validators)
        {
            var result = validator.Validate(model);
            if (result.Failure)
                return result.ToValue<TEntity>();
        }

        var commandContext = await commandContextFactory.CreateAsync<TContext, TModel>(model);
        var entity = creationHandler.Create(commandContext);

        context.GetRepository<TEntity>().Add(entity);

        var saveResult = await context.SaveAsync(token);
        return saveResult.ToValue(entity);
    }
}

public sealed class CreateCommandHandler<TRootEntity, TId, TEntity, TContext, TModel> : ICreateCommandHandler<TEntity, TId, TModel>
    where TRootEntity : class
    where TEntity : class
    where TModel : class
    where TContext : ICommandContext<TRootEntity, TModel>
{
    private readonly IWorkContext context;
    private readonly IEnumerable<IValidator<TModel>> validators;
    private readonly ICommandContextFactory commandContextFactory;
    private readonly ICreationHandler<TRootEntity, TEntity, TContext, TModel> creationHandler;

    public async Task<IOperationResult<TEntity>> HandleAsync(TId id, TModel model, CancellationToken token)
    {
        foreach (var validator in validators)
        {
            var result = validator.Validate(model);
            if (result.Failure)
                return result.ToValue<TEntity>();
        }

        var rootEntity = await context.GetRepository<TRootEntity>().FindAsync(id!);
        if (rootEntity is null)
            return ValueResult.NotFound<TEntity>(
                string.Format(
                    CommandsErrorMessages.NotFoundpattern,
                    GrammarGenre.Get<TRootEntity>(),
                    DisplayNames.Get<TRootEntity>(),
                    id),
                nameof(id));

        var commandContext = await commandContextFactory.CreateAsync<TContext, TRootEntity, TModel>(rootEntity, model);
        var entity = creationHandler.Create(commandContext);

        context.GetRepository<TEntity>().Add(entity);

        var saveResult = await context.SaveAsync(token);
        return saveResult.ToValue(entity);
    }
}