using RoyalCode.DomainEvents;

namespace Microsoft.EntityFrameworkCore;

/// <summary>
/// Extension methods for EF model builder.
/// </summary>
public static class DomainEventModelBuilderExtensions
{
    /// <summary>
    /// <para>
    ///     Ignore the property <see cref="IHasEvents.DomainEvents"/> of every entity of the model
    /// </para>
    /// <para>
    ///     This extension must be used after configure the entity types of the context.
    /// </para>
    /// </summary>
    /// <param name="modelBuilder">EF model builder.</param>
    /// <returns>The same instance of <param name="modelBuilder"></param>.</returns>
    public static ModelBuilder IgnoreDomainEventCollection(this ModelBuilder modelBuilder)
    {
        var propertyNames = typeof(IHasEvents).GetProperties()
            .Select(p => p.Name)
            .ToList();
        
        var entityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(t => typeof(IHasEvents).IsAssignableFrom(t.ClrType));
        
        foreach (var entityType in entityTypes)
        {
            var entityTypeBuilder = modelBuilder.Entity(entityType.ClrType);
            foreach (var propertyName in propertyNames)
                entityTypeBuilder.Ignore(propertyName);
        }
            
        return modelBuilder;
    }
}