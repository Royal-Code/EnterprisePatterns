using RoyalCode.Entities;

namespace RoyalCode.Examples.Blogs.Core.Blogs;

public class Thread : Entity<Guid>
{
    public Thread(string? title, Comment comment)
    {
        Id = Guid.NewGuid();
        Title = title;
        Comments = [comment];
    }

#nullable disable
    /// <summary>
    /// Constructor for deserialization purposes.
    /// </summary>
    public Thread() { }
#nullable enable

    public string? Title { get; set; }

    public IList<Comment> Comments { get; set; }
}
