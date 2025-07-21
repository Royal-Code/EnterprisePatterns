using Microsoft.Extensions.DependencyInjection;
using RoyalCode.SmartProblems;
using RoyalCode.WorkContext.Commands;

namespace RoyalCode.WorkContext.EntityFramework.Internal;

internal sealed class WorkContextCommandRequestDispatcher<TRequest> : CommandRequestDispatcher
    where TRequest : ICommandRequest
{
    public override Task<Result> ExecuteAsync(
        ICommandRequest request,
        IWorkContext workContext,
        IServiceProvider sp,
        CancellationToken ct = default)
    {
        var handler = sp.GetService<ICommandHandler<TRequest>>()
            ?? throw new InvalidOperationException(
                $"The command handler for {typeof(TRequest)} was not found in the service provider");

        return handler.HandleAsync((TRequest)request, workContext, ct);
    }
}

internal sealed class WorkContextCommandRequestDispatcher<TRequest, TResponse> : CommandRequestDispatcher<TResponse>
    where TRequest : ICommandRequest<TResponse>
{
    public override Task<Result<TResponse>> ExecuteAsync(
        ICommandRequest<TResponse> request,
        IWorkContext workContext,
        IServiceProvider sp,
        CancellationToken ct = default)
    {
        var handler = sp.GetService<ICommandHandler<TRequest, TResponse>>()
            ?? throw new InvalidOperationException(
                $"The command handler for {typeof(TRequest)} and response {typeof(TResponse)} was not found in the service provider");

        return handler.HandleAsync((TRequest)request, workContext, ct);
    }
}