using System.Runtime.CompilerServices;

namespace RoyalCode.Commands.Abstractions.Defaults;

internal sealed class DefaultCreationHandler<TService, TEntity, TModel> : ICreationHandler<TEntity, TModel>
    where TEntity : class
    where TModel : class
{
    private readonly TService service;
    private readonly Func<TService, TModel, TEntity> createAction;

    public DefaultCreationHandler(TService service, Func<TService, TModel, TEntity> createAction)
    {
        this.service = service;
        this.createAction = createAction;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TEntity Create(TModel model) => createAction(service, model);
}
