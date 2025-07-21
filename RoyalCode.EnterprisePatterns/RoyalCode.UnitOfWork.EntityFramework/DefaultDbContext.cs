using Microsoft.EntityFrameworkCore;

namespace RoyalCode.UnitOfWork.EntityFramework;

/// <summary>
/// Represents the default Entity Framework <see cref="DbContext"/> used for database operations.
/// </summary>
internal sealed class DefaultDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDbContext"/> class with the specified options.
    /// </summary>
    /// <param name="options">The options to be used by the <see cref="DbContext"/>.</param>
    public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options) { }

    /// <summary>
    /// Configures the model for this context by invoking custom model configuration logic and the base implementation.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        this.ConfigureModelWithServices(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Configures conventions for this context by invoking custom convention configuration logic and the base implementation.
    /// </summary>
    /// <param name="configurationBuilder">The builder used to configure conventions for this context.</param>
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        this.ConfigureConventionsWithServices(configurationBuilder);

        base.ConfigureConventions(configurationBuilder);
    }
}
