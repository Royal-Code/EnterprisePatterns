using Microsoft.EntityFrameworkCore.Infrastructure;
using RoyalCode.UnitOfWork.EntityFramework.Diagnostics.Internal;

namespace Microsoft.EntityFrameworkCore;

/// <summary>
/// Extension methods for <see cref="DbContextOptionsBuilder"/>.
/// </summary>
public static class UnitOfWorkDbContextOptionsBuilderExtensions
{
    /// <summary>
    /// Adds unit of work support.
    /// </summary>
    /// <param name="optionsBuilder">The options builder.</param>
    /// <returns>The same instance of <paramref name="optionsBuilder"/>.</returns>
    public static DbContextOptionsBuilder UseUnitOfWork(
        this DbContextOptionsBuilder optionsBuilder)
    {
        var extension = optionsBuilder.Options.FindExtension<UnitOfWorkDbContextOptionsExtension>();
        if (extension is null)
        {
            extension = new UnitOfWorkDbContextOptionsExtension();
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
        }

        return optionsBuilder;
    }
}