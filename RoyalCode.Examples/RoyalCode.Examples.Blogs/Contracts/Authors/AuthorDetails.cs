using RoyalCode.Examples.Blogs.Core.Support;
using RoyalCode.SmartCommands;

namespace RoyalCode.Examples.Blogs.Contracts.Authors;

/// <summary>
/// A class representing the details of an author.
/// </summary>
[MapGroup("author")]
[MapFind("{id}", "find-author-by-id"), EntityReference<Author, Guid>]
[WithSummary("Author Details")]
[WithDescription("Represents the details of an author, including their unique identifier, name, email, and creation date.")]
public class AuthorDetails
{
    /// <summary>
    /// The unique identifier of the author.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the author.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The email of the author.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// The date and time when the author was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
