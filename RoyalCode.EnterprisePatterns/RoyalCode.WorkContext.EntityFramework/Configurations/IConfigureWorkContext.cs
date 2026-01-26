using Microsoft.EntityFrameworkCore;

namespace RoyalCode.WorkContext.EntityFramework.Configurations;

/// <summary>
/// Defines a contract for configuring a work context builder for a specific Entity Framework database context type.
/// </summary>
/// <remarks>
///     Implementations of this interface can be used to customize the setup or behavior of work contexts in
///     applications that use Entity Framework.
/// </remarks>
/// <typeparam name="TDbContext">
///     The type of the Entity Framework database context to be used with the work context builder.
///     Must derive from <see cref="DbContext"/>.
/// </typeparam>
public interface IConfigureWorkContext<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    /// Applies custom configurations to the provided work context builder.
    /// </summary>
    /// <param name="builder">The work context builder to be configured.</param>
    /// <returns>The configured work context builder.</returns>
    public IWorkContextBuilder<TDbContext> ConfigureWorkContext(IWorkContextBuilder<TDbContext> builder);
}
