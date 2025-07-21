using RoyalCode.SmartProblems;
using RoyalCode.WorkContext;
using RoyalCode.WorkContext.Commands;

namespace RoyalCode.WorkContext.EntityFramework.Internal;

internal abstract class CommandRequestDispatcher
{
    public abstract Task<Result> ExecuteAsync(
        ICommandRequest request,
        IWorkContext workContext,
        IServiceProvider sp,
        CancellationToken ct = default);
}

internal abstract class CommandRequestDispatcher<TResponse>
{

    public abstract Task<Result<TResponse>> ExecuteAsync(
        ICommandRequest<TResponse> request,
        IWorkContext workContext,
        IServiceProvider sp,
        CancellationToken ct = default);
}