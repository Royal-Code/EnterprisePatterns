using RoyalCode.Entities;
using RoyalCode.Examples.Blogs.Core.Support;

namespace RoyalCode.Examples.Blogs.Core.Blogs;

public class Blog : Entity<Guid>
{
    public string Title { get; set; }

    public string Content { get; set; }

    public DateTime CreatedDate { get; set; }

    public Author Owner { get; set; }

    public ICollection<Author> Authors { get; set; } = [];

    public ICollection<Post> Posts { get; set; } = [];
}
