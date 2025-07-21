using RoyalCode.Entities;
using RoyalCode.Examples.Blogs.Core.Support;

namespace RoyalCode.Examples.Blogs.Core.Blogs;

public class Post : Entity<Guid>
{
    public Post(Blog blog, string title, string content, Author author, IEnumerable<string>? tags)
    {
        Id = Guid.CreateVersion7();
        Blog = blog;
        Title = title;
        Content = content;
        CreatedDate = DateTime.UtcNow;
        Author = author;

        if (tags is not null)
            Tags = [.. tags];
        else
            Tags = [];
    }

#nullable disable
    /// <summary>
    /// Constructor for deserialization purposes.
    /// </summary>
    public Post() { }
#nullable enable

    public Blog Blog { get; set; }

    public string Title { get; set; }
    
    public string Content { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public Author Author { get; set; }

    public ICollection<string> Tags { get; set; }

    public ICollection<Thread> Threads { get; set; } = [];
}
