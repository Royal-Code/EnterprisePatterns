using Microsoft.EntityFrameworkCore;
using RoyalCode.Examples.Blogs.Core.Blogs;
using RoyalCode.Examples.Blogs.Core.Support;
using RoyalCode.WorkContext.EntityFramework.Configurations;

namespace RoyalCode.Examples.Blogs.Infra.Persistence;

public static class ConfigureWorkContext
{

    public static IWorkContextBuilder<TDbContext> ConfigureBlogs<TDbContext>(this IWorkContextBuilder<TDbContext> builder)
        where TDbContext : DbContext
    {
        builder.ConfigureRepositories(b =>
        {
            b.Add<Blog>()
                .Add<Post>()
                .Add<Core.Blogs.Thread>()
                .Add<Comment>()
                .Add<Author>();
        });
        
        return builder;
    }
}
