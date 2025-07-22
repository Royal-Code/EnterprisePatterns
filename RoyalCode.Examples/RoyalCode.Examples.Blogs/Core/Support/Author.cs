using RoyalCode.Entities;

namespace RoyalCode.Examples.Blogs.Core.Support;

public class Author : Entity<Guid>
{
    public Author(string name, string email)
    {
        Id = Guid.CreateVersion7();
        Name = name;
        Email = email;
        CreatedAt = DateTime.UtcNow;
    }

#nullable disable
    /// <summary>
    /// Constructor for deserialization purposes.
    /// </summary>
    public Author() { }
#nullable enable

    public string Name { get; set; }
    
    public string Email { get; set; }

    public bool IsConfirmed { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public void ConfirmAuthor() 
    {
        IsConfirmed = true;
        LastModifiedDate = DateTime.UtcNow;
    }
}
