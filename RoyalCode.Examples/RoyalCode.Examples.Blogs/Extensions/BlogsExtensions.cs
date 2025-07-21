using RoyalCode.SmartCommands;

namespace Microsoft.Extensions.DependencyInjection;

[AddHandlersServices("Blogs")]
public static partial class BlobsExtensions
{
    public static IServiceCollection AddBlobs(this IServiceCollection services)
    {
        services.AddBlogsHandlersServices();

        return services;
    }
}
