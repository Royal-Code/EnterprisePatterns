using Microsoft.AspNetCore.Routing;
using RoyalCode.SmartCommands;

namespace RoyalCode.Examples.Blogs.Api;

[MapApiHandlers]
public static partial class BlogsEndpoints
{
    public static void MapBlob(this IEndpointRouteBuilder app)
    {
        app.MapHelloGroup();
        app.MapAuthorsGroup();
    }
}
