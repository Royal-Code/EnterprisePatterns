namespace RoyalCode.Examples.Blogs.Contracts.Authors;

/// <summary>
/// A class representing the details of an author.
/// </summary>
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
