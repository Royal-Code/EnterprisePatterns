using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace RoyalCode.Persistence.EntityFramework.UnitOfWork.Diagnostics.Internal;

/// <summary>
/// <para>
///     Internal class for extends the <see cref="DbContext"/>.
/// </para>
/// </summary>
public class UnitOfWorkDbContextOptionsExtension : IDbContextOptionsExtension
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public UnitOfWorkDbContextOptionsExtension()
    {
        Info = new UnitOfWorkDbDbContextOptionsExtensionInfo(this);
    }
    
    /// <summary>
    /// <para>
    ///     Adds the services for extends the <see cref="DbContext"/>.
    /// </para>
    /// </summary>
    /// <param name="services"></param>
    public void ApplyServices(IServiceCollection services)
    {
        services.AddTransient<IInterceptorAggregator, UnitOfWorkInitializeInterceptorAggregator>();
    }

    /// <summary>
    /// Nothing.
    /// </summary>
    /// <param name="options"></param>
    public void Validate(IDbContextOptions options) { }

    /// <summary>
    /// Extension info.
    /// </summary>
    public DbContextOptionsExtensionInfo Info { get; }
}