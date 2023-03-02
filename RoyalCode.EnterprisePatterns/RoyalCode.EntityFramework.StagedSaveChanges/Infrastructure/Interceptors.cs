using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace RoyalCode.EntityFramework.StagedSaveChanges.Infrastructure;

#pragma warning disable EF1001 // Internal EF Core API usage.

internal static class Interceptors
{
    private static readonly Dictionary<Type, IStagedSaveChangesInterceptor> interceptors = new();

    public static IStagedSaveChangesInterceptor GetStagedSaveChangesInterceptor(DbContext db)
    {
        if (interceptors.TryGetValue(db.GetType(), out var interceptor))
            return interceptor;

        interceptor = ((IDbContextDependencies)db).UpdateLogger.Interceptors
            ?.Aggregate<IStagedSaveChangesInterceptor>()
            ?? throw new TransactionManagerInitializationException();

        interceptors[db.GetType()] = interceptor;

        return interceptor;
    }
}

#pragma warning restore EF1001 // Internal EF Core API usage.