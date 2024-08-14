using Microsoft.Extensions.DependencyInjection;

namespace RoyalCode.Events.Outbox.Abstractions.Services.Defaults;

/// <summary>
/// Extensions methods.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds the default outbox services. 
    /// For correct operation, you'll need to add implementations of the contract handlers.
    /// </summary>
    /// <param name="services"></param>
    public static void AddOutboxDefaultCoreServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (services.Any(d => d.ServiceType == typeof(IOutboxService)))
            return;

        services.AddTransient<IOutboxService, OutboxService>();
        services.AddTransient<IMessageDispatcher, MessageDispatcher>();
        services.AddTransient(typeof(MessageDispatcher<>));
    }
}
