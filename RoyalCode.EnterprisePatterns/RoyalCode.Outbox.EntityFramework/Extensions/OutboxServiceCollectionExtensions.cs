using Microsoft.EntityFrameworkCore;
using RoyalCode.Outbox.Abstractions.Contracts.Handlers;
using RoyalCode.Outbox.Abstractions.Contracts;
using RoyalCode.Outbox.Abstractions.Options;
using RoyalCode.Outbox.Abstractions.Services.Defaults;
using RoyalCode.Outbox.Abstractions.Services;
using RoyalCode.Outbox.EntityFramework.Services.Handlers;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class OutboxServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Outbox services to EntityFramework.
    /// </summary>
    /// <typeparam name="TDbContext">The <see cref="DbContext"/> type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The same instance of <paramref name="services"/> for chained calls.</returns>
    public static IServiceCollection AddOutboxServices<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        if (services.Any(d => d.ServiceType == typeof(OutboxService)))
            return services;

        services.AddTransient<IMessageDispatcher, MessageDispatcher>();
        services.AddTransient(typeof(MessageDispatcher<>));
        services.AddTransient<OutboxService>();

        services.AddTransient<IRegisterConsumerHandler, RegisterConsumerHandler<TDbContext>>();
        services.AddTransient<IGetMessagesHandler, GetMessagesHandler<TDbContext>>();
        services.AddTransient<ICommitConsumedHandler, CommitConsumedHandler<TDbContext>>();
        services.AddTransient<ICreateMessageHandler, CreateMessageHandler<TDbContext>>();

        return services;
    }

    /// <summary>
    /// Configures the options of the <see cref="OutboxOptions"/>.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <param name="configureAction">Action for configuration.</param>
    /// <returns>The same instance of <see cref="IServiceCollection"/> for chained calls.</returns>
    public static IServiceCollection ConfigureOutbox(this IServiceCollection services,
        Action<OutboxOptions> configureAction)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureAction);

        return services.Configure(configureAction);
    }
}
