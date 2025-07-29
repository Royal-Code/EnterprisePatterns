using RoyalCode.Examples.Blogs.Core.Support;
using RoyalCode.SmartCommands;
using RoyalCode.SmartProblems;
using RoyalCode.SmartProblems.Entities;
using RoyalCode.SmartSelector;
using RoyalCode.WorkContext;

namespace RoyalCode.Examples.Blogs.Contracts.Authors;

[AutoSelect<EmailVerification>]
public partial class AuthorEmailVerificationDetails
{
    public string AuthorName { get; set; }

    public string LinkCode { get; set; }

    public string UICode { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ValidUntil { get; set; }

    public DateTimeOffset? ValidatedAt { get; set; }

    [WithUnitOfWork<IWorkContext>]
    public static IEnumerable<AuthorEmailVerificationDetails> Query([WithParameter] Author author)
    {
        return author.EmailVerifications.SelectAuthorEmailVerificationDetails();
    }
}

public interface IAuthorEmailVerificationDetailsQueryHandler
{
    Task<Result<IEnumerable<AuthorEmailVerificationDetails>>> HandleAsync(Id<Author, Guid> id, CancellationToken ct);
}

public class AuthorEmailVerificationDetailsQueryHandler : IAuthorEmailVerificationDetailsQueryHandler
{
    private readonly IUnitOfWorkAccessor<IWorkContext> accessor;

    public AuthorEmailVerificationDetailsQueryHandler(IUnitOfWorkAccessor<IWorkContext> accessor)
    {
        this.accessor = accessor;
    }

    public async Task<Result<IEnumerable<AuthorEmailVerificationDetails>>> HandleAsync(Id<Author, Guid> id, CancellationToken ct)
    {
        var findResult = await accessor.FindEntityAsync(id, ct);
        if (findResult.NotFound(out var notFoundProblem))
            return notFoundProblem;

        var result = AuthorEmailVerificationDetails.Query(findResult.Entity);

        return new(result);
    }
}