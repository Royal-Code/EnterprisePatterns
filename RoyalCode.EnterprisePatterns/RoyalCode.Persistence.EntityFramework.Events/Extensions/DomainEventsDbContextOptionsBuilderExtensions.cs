using RoyalCode.Persistence.EntityFramework.Events;

namespace Microsoft.EntityFrameworkCore;

/// <summary>
/// Extension methods for <see cref="DbContextOptionsBuilder"/>.
/// </summary>
public static class DomainEventsDbContextOptionsBuilderExtensions
{
    /// <summary>
    /// Adds the components for handle domain events.
    /// </summary>
    /// <param name="optionsBuilder">The options builder.</param>
    /// <returns>The same instance of <paramref name="optionsBuilder"/>.</returns>
    public static DbContextOptionsBuilder UseDomainEventHandler(
        this DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new DomainEventUnitOfWorkInterceptor());
        return optionsBuilder;
    }
}