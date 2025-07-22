using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoyalCode.Examples.Blogs.Core.Blogs;
using RoyalCode.Examples.Blogs.Core.Support;
using RoyalCode.WorkContext.EntityFramework.Configurations;

namespace RoyalCode.Examples.Blogs.Infra.Persistence;

public static class ConfigureWorkContext
{
    public static IWorkContextBuilder<TDbContext> ConfigureBlogs<TDbContext>(this IWorkContextBuilder<TDbContext> builder)
        where TDbContext : DbContext
    {
        builder
            .ConfigureModel(b =>
            {
                b.ApplyConfigurationsFromAssembly(typeof(ConfigureWorkContext).Assembly);
            })
            .ConfigureRepositories(b =>
            {
                b.Add<Blog>()
                    .Add<Post>()
                    .Add<Core.Blogs.Thread>()
                    .Add<Comment>()
                    .Add<Author>();
            })
            .ConfigureSearches(b =>
            {
                b.Add<Author>();
            });
        
        return builder;
    }
}

public class BlogMapping : IEntityTypeConfiguration<Blog>
{
    public void Configure(EntityTypeBuilder<Blog> builder)
    {
        builder
            .HasMany(b => b.Posts)
            .WithOne(p => p.Blog)
            .HasForeignKey("BlogId")
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(b => b.Owner)
            .WithMany()
            .HasForeignKey("OwnerId");

        builder
            .HasMany(b => b.Authors)
            .WithMany();
    }
}

public class PostMapping : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder
            .HasMany(p => p.Threads)
            .WithOne()
            .HasForeignKey("PostId")
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(p => p.Author)
            .WithMany()
            .HasForeignKey("AuthorId");

    }
}

public class ThreadMapping : IEntityTypeConfiguration<Core.Blogs.Thread>
{
    public void Configure(EntityTypeBuilder<Core.Blogs.Thread> builder)
    {
        builder
            .HasMany(t => t.Comments)
            .WithOne()
            .HasForeignKey("ThreadId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class CommentMapping : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder
            .HasOne(c => c.Author)
            .WithMany()
            .HasForeignKey("AuthorId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AuthorMapping : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        
    }
}