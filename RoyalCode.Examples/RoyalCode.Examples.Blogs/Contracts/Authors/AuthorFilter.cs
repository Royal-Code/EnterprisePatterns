namespace RoyalCode.Examples.Blogs.Contracts.Authors;

public class AuthorFilter
{
    /// <summary>
    /// The name of the author to filter by.
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// The email of the author to filter by.
    /// </summary>
    /// 
    public string? Email { get; set; }
    /// <summary>
    /// The page number for pagination.
    /// </summary>
}
