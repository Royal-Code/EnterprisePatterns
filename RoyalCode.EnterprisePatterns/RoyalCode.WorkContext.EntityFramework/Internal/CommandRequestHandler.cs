using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.SmartProblems;
using RoyalCode.WorkContext.Abstractions.Commands;
using RoyalCode.WorkContext.EntityFramework.Commands;
using System.Collections.Concurrent;

namespace RoyalCode.WorkContext.EntityFramework.Internal;

internal static class CommandRequestHandler
{
    private static readonly ConcurrentDictionary<Type, object> handlers = new();

    public static Task<Result> ExecuteAsync<TDbContext>(
        ICommandRequest request,
        IWorkContext<TDbContext> workContext,
        IServiceProvider sp,
        CancellationToken ct = default)
        where TDbContext : DbContext
    {
        var requestType = request.GetType();

        var handler = (CommandRequestHandler<TDbContext>)handlers.GetOrAdd(requestType, type =>
        {
            var handlerType = typeof(DefaultCommandRequestHandler<,>).MakeGenericType(typeof(TDbContext), type);
            return Activator.CreateInstance(handlerType) ??
                   throw new InvalidOperationException(
                       $"Cannot create an instance of command request handler for {requestType}");
        });

        return handler.ExecuteAsync(request, workContext, sp, ct);
    }

    public static Task<Result<TResponse>> ExecuteAsync<TDbContext, TResponse>(
        ICommandRequest<TResponse> request,
        IWorkContext<TDbContext> workContext,
        IServiceProvider sp,
        CancellationToken ct = default)
        where TDbContext : DbContext
    {
        var requestType = request.GetType();

        var handler = (CommandRequestHandler<TDbContext, TResponse>)handlers.GetOrAdd(requestType, type =>
        {
            var handlerType = typeof(DefaultCommandRequestHandler<,,>).MakeGenericType(typeof(TDbContext), type, typeof(TResponse));
            return Activator.CreateInstance(handlerType) ??
                   throw new InvalidOperationException(
                       $"Cannot create an instance of command request handler for {requestType} and response {typeof(TResponse)}");
        });

        return handler.ExecuteAsync(request, workContext, sp, ct);
    }
}

internal abstract class CommandRequestHandler<TDbContext>
    where TDbContext : DbContext
{
    public abstract Task<Result> ExecuteAsync(
        ICommandRequest request,
        IWorkContext<TDbContext> workContext,
        IServiceProvider sp,
        CancellationToken ct = default);
}

internal sealed class DefaultCommandRequestHandler<TDbContext, TRequest> : CommandRequestHandler<TDbContext>
    where TDbContext : DbContext
    where TRequest : ICommandRequest
{
    public override Task<Result> ExecuteAsync(
        ICommandRequest request,
        IWorkContext<TDbContext> workContext,
        IServiceProvider sp,
        CancellationToken ct = default)
    {
        var handler = sp.GetService<ICommandHandler<TDbContext, TRequest>>()
            ?? throw new InvalidOperationException(
                $"The command handler for {typeof(TRequest)} was not found in the service provider");

        return handler.HandleAsync((TRequest)request, workContext, ct);
    }
}

internal abstract class CommandRequestHandler<TDbContext, TResponse>
    where TDbContext : DbContext
{
    
    public abstract Task<Result<TResponse>> ExecuteAsync(
        ICommandRequest<TResponse> request,
        IWorkContext<TDbContext> workContext,
        IServiceProvider sp,
        CancellationToken ct = default);
}

internal sealed class DefaultCommandRequestHandler<TDbContext, TRequest, TResponse> : CommandRequestHandler<TDbContext, TResponse>
    where TDbContext : DbContext
    where TRequest : ICommandRequest<TResponse>
{
    public override Task<Result<TResponse>> ExecuteAsync(
        ICommandRequest<TResponse> request,
        IWorkContext<TDbContext> workContext,
        IServiceProvider sp,
        CancellationToken ct = default)
    {
        var handler = sp.GetService<ICommandHandler<TDbContext, TRequest, TResponse>>()
            ?? throw new InvalidOperationException(
                $"The command handler for {typeof(TRequest)} and response {typeof(TResponse)} was not found in the service provider");

        return handler.HandleAsync((TRequest)request, workContext, ct);
    }
}