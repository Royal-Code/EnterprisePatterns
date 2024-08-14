using Microsoft.EntityFrameworkCore;
using RoyalCode.DomainEvents;
using RoyalCode.Events.Outbox.Abstractions.Models;

namespace RoyalCode.Events.Outbox.EntityFramework.Postgres.Mappings;

/// <summary>
/// Configurations for Outbox models and entities.
/// </summary>
public static class OutboxConfigurations
{
    /// <summary>
    /// Apply the configurations for the outbox entities.
    /// </summary>
    /// <param name="builder">The <see cref="ModelBuilder"/>.</param>
    /// <param name="schema">Optional, the schema name.</param>
    public static void ApplyOutboxConfigurations(this ModelBuilder builder, string? schema = null)
    {
        builder.IgnoreHasEvents();

        builder.Entity<OutboxMessage>(b =>
        {
            if (schema is not null)
                b.ToTable("outbox_messages", schema);
            else
                b.ToTable("outbox_messages");

            b.HasKey(k => k.Id);

            b.Property(k => k.Id).HasColumnName("id");
            b.Property(k => k.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz");
            b.Property(k => k.MessageType).HasColumnName("message_type").HasMaxLength(100).IsRequired();
            b.Property(k => k.VersionType).HasColumnName("version_type").IsRequired();
            b.Property(k => k.Key).HasColumnName("key");
            b.Property(k => k.Payload).HasColumnName("payload").HasColumnType("text").IsRequired();
        });

        builder.Entity<OutboxConsumer>(b =>
        {
            if (schema is not null)
                b.ToTable("outbox_consumers", schema);
            else
                b.ToTable("outbox_consumers");

            b.HasKey(k => k.Id);
            b.HasIndex(k => k.Name, "ux_outbox_consumers_name").IsUnique();

            b.Property(k => k.Id).HasColumnName("id");
            b.Property(k => k.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            b.Property(k => k.LastConsumedMessageId).HasColumnName("last_consumed_message_id");
        });
    }

    /// <summary>
    /// <para>
    ///     Configures the EFCore model to not map the properties of <see cref="IHasEvents.DomainEvents"/>.
    /// </para>
    /// <para>
    ///     This method is called by <see cref="ApplyOutboxConfigurations"/>. 
    ///     If the <see cref="ApplyOutboxConfigurations"/> is called 
    ///     at the end of the <see cref="ModelBuilder"/> configurations, 
    ///     this rule will be applied to all entities 
    ///     and it will not be necessary to call this method during the <see cref="DbContext"/> configuration.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="ModelBuilder"/>.</param>
    public static void IgnoreHasEvents(this ModelBuilder builder)
    {
        builder.Model.GetEntityTypes()
            .Where(static e => typeof(IHasEvents).IsAssignableFrom(e.ClrType))
            .Each(builder, static (e, b) => b.Entity(e.ClrType).Ignore(nameof(IHasEvents.DomainEvents)));
    }

    private static void Each<TItem, THandler>(
        this IEnumerable<TItem> enumerable,
        THandler handler,
        Action<TItem, THandler> action)
    {
        foreach (TItem item in enumerable)
            action(item, handler);
    }
}
