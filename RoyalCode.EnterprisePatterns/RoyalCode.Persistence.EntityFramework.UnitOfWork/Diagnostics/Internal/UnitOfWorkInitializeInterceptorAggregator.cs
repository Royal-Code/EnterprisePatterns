using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RoyalCode.Persistence.EntityFramework.UnitOfWork.Interceptors;

namespace RoyalCode.Persistence.EntityFramework.UnitOfWork.Diagnostics.Internal;

/// <summary>
/// <para>
///     Internal class for extends the <see cref="DbContext"/>.
/// </para>
/// </summary>
public class UnitOfWorkInitializeInterceptorAggregator : IInterceptorAggregator
{
    /// <inheritdoc />
    public IInterceptor AggregateInterceptors(IReadOnlyList<IInterceptor> interceptors)
    {
        return new UnitOfWorkInterceptorAggregatorExecutor(interceptors);
    }

    /// <inheritdoc />
    public Type InterceptorType => typeof(IUnitOfWorkInterceptor);
}