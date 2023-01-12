
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using RoyalCode.Persistence.EntityFramework.UnitOfWork.Exceptions;
using RoyalCode.Persistence.EntityFramework.UnitOfWork.Interceptors;

namespace RoyalCode.Persistence.EntityFramework.UnitOfWork.Diagnostics.Internal;

internal static class Interceptors<TDbContext>
    where TDbContext : DbContext
{
    private static IUnitOfWorkInterceptor? unitOfWorkInterceptor;

    public static IUnitOfWorkInterceptor GetUnitOfWorkInterceptor(TDbContext db)
    {
        return unitOfWorkInterceptor ??= ((IDbContextDependencies)db).UpdateLogger.Interceptors
                ?.Aggregate<IUnitOfWorkInterceptor>()
                ?? throw new UnitOfWorkInitializationException();
    }
}
