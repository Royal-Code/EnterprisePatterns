using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace RoyalCode.Persistence.EntityFramework.UnitOfWork.Diagnostics.Internal;

/// <summary>
/// <para>
///     Internal class for extends the <see cref="DbContext"/>.
/// </para>
/// </summary>
public class UnitOfWorkInitializeInterceptorAggregatorExecutor : IUnitOfWorkInitializeInterceptor
{
    private readonly IReadOnlyList<IInterceptor> interceptors;

    /// <summary>
    /// Create a new interceptor aggregate executor.
    /// </summary>
    /// <param name="interceptors">The interceptors to be executed.</param>
    public UnitOfWorkInitializeInterceptorAggregatorExecutor(IReadOnlyList<IInterceptor> interceptors)
    {
        this.interceptors = interceptors;
    }

    /// <summary>
    /// Execute the interceptions on initializing.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> of the unit of work.</param>
    public void Initializing(DbContext context)
    {
        foreach (var i in interceptors.OfType<IUnitOfWorkInitializeInterceptor>())
        {
            i.Initializing(context);
        }
    }
}