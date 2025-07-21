using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.SmartProblems;
using RoyalCode.WorkContext.Commands;
using System.Collections.Concurrent;

namespace RoyalCode.WorkContext.EntityFramework.Internal;

internal class CommandDispatcher : ICommandDispatcher
{
    private static readonly ConcurrentDictionary<Type, object> handlers = new();

    private readonly IServiceProvider serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public Task<Result> SendAsync(ICommandRequest request, CancellationToken ct = default)
    {
        var requestType = request.GetType();

        var handler = (ICommandRequestDispatcher)handlers.GetOrAdd(requestType, static (type, sp) =>
        {
            var dispatcherType = typeof(IServiceCommandRequestDispatcher<>).MakeGenericType(type);
            return sp.GetService(dispatcherType)
                ?? throw new InvalidOperationException(
                    $"The command request dispatcher for {type} was not found in the service provider");

        }, serviceProvider);

        return handler.ExecuteAsync(request, serviceProvider, ct);
    }

    public Task<Result<TResponse>> SendAsync<TResponse>(ICommandRequest<TResponse> request, CancellationToken ct = default)
    {
        var requestType = request.GetType();

        var handler = (ICommandRequestDispatcher<TResponse>)handlers.GetOrAdd(requestType, static (type, sp) =>
        {
            var dispatcherType = typeof(IServiceCommandRequestDispatcher<,>).MakeGenericType(type, typeof(TResponse));
            return sp.GetService(dispatcherType)
                ?? throw new InvalidOperationException(
                    $"The command request dispatcher for {type} and response {typeof(TResponse)} was not found in the service provider");
        }, serviceProvider);

        return handler.ExecuteAsync(request, serviceProvider, ct);
    }
}

internal interface ICommandRequestDispatcher
{
    public abstract Task<Result> ExecuteAsync(
        ICommandRequest request,
        IServiceProvider sp,
        CancellationToken ct = default);
}

internal interface IServiceCommandRequestDispatcher<TRequest> : ICommandRequestDispatcher
    where TRequest : ICommandRequest
{ }

internal interface ICommandRequestDispatcher<TResponse>
{
    public abstract Task<Result<TResponse>> ExecuteAsync(
        ICommandRequest<TResponse> request,
        IServiceProvider sp,
        CancellationToken ct = default);
}

internal interface IServiceCommandRequestDispatcher<TRequest, TResponse> : ICommandRequestDispatcher<TResponse>
    where TRequest : ICommandRequest<TResponse>
{ }

internal sealed class DefaultServiceCommandRequestDispatcher<TDbContext, TRequest> : IServiceCommandRequestDispatcher<TRequest>
    where TDbContext : DbContext
    where TRequest : ICommandRequest
{
    public Task<Result> ExecuteAsync(
        ICommandRequest request,
        IServiceProvider sp,
        CancellationToken ct = default)
    {
        var handler = sp.GetService<ICommandHandler<TRequest>>()
            ?? throw new InvalidOperationException(
                $"The command handler for {typeof(TRequest)} was not found in the service provider");

        var workContext = sp.GetService<IWorkContext<TDbContext>>()
            ?? throw new InvalidOperationException(
                $"The work context for {typeof(TDbContext)} was not found in the service provider");

        return handler.HandleAsync((TRequest)request, workContext, ct);
    }
}

internal sealed class DefaultServiceCommandRequestDispatcher<TDbContext, TRequest, TResponse> : IServiceCommandRequestDispatcher<TRequest, TResponse>
    where TDbContext : DbContext
    where TRequest : ICommandRequest<TResponse>
{
    public Task<Result<TResponse>> ExecuteAsync(
        ICommandRequest<TResponse> request,
        IServiceProvider sp,
        CancellationToken ct = default)
    {
        var handler = sp.GetService<ICommandHandler<TRequest, TResponse>>()
            ?? throw new InvalidOperationException(
                $"The command handler for {typeof(TRequest)} and response {typeof(TResponse)} was not found in the service provider");

        var workContext = sp.GetService<IWorkContext<TDbContext>>()
            ?? throw new InvalidOperationException(
                $"The work context for {typeof(TDbContext)} was not found in the service provider");

        return handler.HandleAsync((TRequest)request, workContext, ct);
    }
}