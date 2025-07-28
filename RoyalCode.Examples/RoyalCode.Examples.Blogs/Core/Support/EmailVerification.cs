using RoyalCode.Entities;
using RoyalCode.SmartProblems;
using System.Text;

namespace RoyalCode.Examples.Blogs.Core.Support;

public class EmailVerification : Entity<Guid>
{

    public EmailVerification(Author author)
    {
        Id = Guid.CreateVersion7();
        LinkCode = Guid.NewGuid().ToString("N");

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 6; i++)
        {
            sb.Append(Random.Shared.Next(0, 10));
        }
        UICode = sb.ToString();

        Author = author;
        CreatedAt = DateTimeOffset.UtcNow;
        ValidUntil = CreatedAt.AddDays(1);
    }

#nullable disable
    /// <summary>
    /// Constructor for deserialization purposes.
    /// </summary>
    public EmailVerification() { }
#nullable enable

    public string LinkCode { get; set; }

    public string UICode { get; set; }

    public Author Author { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ValidUntil { get; set; }

    public DateTimeOffset? ValidatedAt { get; set; }

    public Result TryValidate(string code)
    {
        if (ValidatedAt.HasValue)
        {
            return Problems.InvalidState("Email already validated.");
        }
        
        if (UICode != code)
        {
            return Problems.InvalidParameter("Invalid verification code.");
        }
        
        if (DateTimeOffset.UtcNow > ValidUntil)
        {
            return Problems.InvalidState("Verification code expired.");
        }

        ValidatedAt = DateTimeOffset.UtcNow;
        Author.ConfirmAuthor();
        
        return Result.Ok();
    }
}
