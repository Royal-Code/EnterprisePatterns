using RoyalCode.Entities;
using RoyalCode.Examples.Blogs.Core.Support;

namespace RoyalCode.Examples.Blogs.Core.Blogs;

public class Comment : Entity<Guid>
{
    public Comment(string content, Author author)
    {
        Id = Guid.CreateVersion7();
        Content = content;
        CreatedDate = DateTime.UtcNow;
        Author = author;
    }

#nullable disable
    /// <summary>
    /// Constructor for deserialization purposes.
    /// </summary>
    public Comment() { }
#nullable enable

    public string Content { get; set; }

    public DateTime CreatedDate { get; set; }

    public Author Author { get; set; }
}
