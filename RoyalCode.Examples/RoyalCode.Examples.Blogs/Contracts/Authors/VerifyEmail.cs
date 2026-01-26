using RoyalCode.Examples.Blogs.Core.Support;
using RoyalCode.SmartCommands;
using RoyalCode.SmartProblems;
using RoyalCode.SmartValidations;
using RoyalCode.WorkContext;
using System.Diagnostics.CodeAnalysis;

namespace RoyalCode.Examples.Blogs.Contracts.Authors;

[MapGroup("author")]
[MapPost("verify-email", "author-verify-email")]
[WithSummary("Verify Email")]
[WithDescription("Verifies the email of the author using the verification code sent via email.")]
public partial class VerifyEmail : IValidable
{
    /// <summary>
    /// Code to verify the email sent to the user via email link.
    /// </summary>
    public string? LinkCode { get; set; }

    /// <summary>
    /// Code to verify the email informed by the user in the UI.
    /// </summary>
    public string? UICode { get; set; }

    /// <summary>
    /// Validates the email verification codes.
    /// </summary>
    /// <param name="problems">Returns problems if any validation fails.</param>
    /// <returns>True if has problems, false otherwise (valid).</returns>
    [MemberNotNullWhen(false, nameof(LinkCode), nameof(UICode))]
    public bool HasProblems([NotNullWhen(true)] out Problems? problems)
    {
        return Rules.Set<VerifyEmail>()
            .NotNull(LinkCode)
            .NotNull(UICode)
            .HasProblems(out problems);
    }

    [Command, WithValidateModel, WithUnitOfWork<IWorkContext>]
    [ProduceProblems(ProblemCategory.InvalidParameter, ProblemCategory.InvalidState)]
    internal async Task<Result> Execute(IWorkContext context)
    {
        WasValidated();
        return await context.Repository<EmailVerification>().FindAsync(e => e.LinkCode == LinkCode)
            .ContinueAsync(emailVerification =>
            {
                return emailVerification.TryValidate(UICode);
            });
    }
}
