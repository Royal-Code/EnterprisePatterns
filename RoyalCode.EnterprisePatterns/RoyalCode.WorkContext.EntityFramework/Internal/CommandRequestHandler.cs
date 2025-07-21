using Microsoft.EntityFrameworkCore;
using RoyalCode.SmartProblems;
using RoyalCode.WorkContext.Commands;
using System.Collections.Concurrent;

namespace RoyalCode.WorkContext.EntityFramework.Internal;

internal static class CommandRequestHandler<TDbContext>
    where TDbContext : DbContext
{
    private static readonly ConcurrentDictionary<Type, object> handlers = new();

    public static Task<Result> ExecuteAsync(
        ICommandRequest request,
        IWorkContext<TDbContext> workContext,
        IServiceProvider sp,
        CancellationToken ct = default)
    {
        var requestType = request.GetType();

        var handler = (CommandRequestDispatcher)handlers.GetOrAdd(requestType, static type =>
        {
            var handlerType = typeof(WorkContextCommandRequestDispatcher<>).MakeGenericType(type);
            return Activator.CreateInstance(handlerType) ??
                   throw new InvalidOperationException(
                       $"Cannot create an instance of command request handler for {type}");
        });

        return handler.ExecuteAsync(request, workContext, sp, ct);
    }

    public static Task<Result<TResponse>> ExecuteAsync<TResponse>(
        ICommandRequest<TResponse> request,
        IWorkContext<TDbContext> workContext,
        IServiceProvider sp,
        CancellationToken ct = default)
    {
        var requestType = request.GetType();

        var handler = (CommandRequestDispatcher<TResponse>)handlers.GetOrAdd(requestType, static type =>
        {
            var handlerType = typeof(WorkContextCommandRequestDispatcher<,>).MakeGenericType(type, typeof(TResponse));
            return Activator.CreateInstance(handlerType) ??
                   throw new InvalidOperationException(
                       $"Cannot create an instance of command request handler for {type} and response {typeof(TResponse)}");
        });

        return handler.ExecuteAsync(request, workContext, sp, ct);
    }
}