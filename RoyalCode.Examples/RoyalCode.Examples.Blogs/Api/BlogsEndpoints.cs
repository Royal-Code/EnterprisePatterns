using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RoyalCode.Examples.Blogs.Contracts.Authors;
using RoyalCode.Examples.Blogs.Core.Support;
using RoyalCode.SmartCommands;

namespace RoyalCode.Examples.Blogs.Api;

[MapApiHandlers]
public static partial class BlogsEndpoints
{
    public static void MapBlob(this IEndpointRouteBuilder app)
    {
        app.MapHelloGroup();

        var authors = app.MapAuthorsGroup();

        authors.MapSearch<Author, AuthorDetails, AuthorFilter>("")
            .WithDescription("Searches for authors based on the provided filter criteria.")
            .WithName("SearchAuthors")
            .WithOpenApi();
    }
}
