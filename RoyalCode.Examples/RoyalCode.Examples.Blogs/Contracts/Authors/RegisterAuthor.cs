using RoyalCode.Examples.Blogs.Core.Support;
using RoyalCode.SmartCommands;
using RoyalCode.SmartProblems;
using RoyalCode.SmartValidations;
using RoyalCode.WorkContext;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace RoyalCode.Examples.Blogs.Contracts.Authors;

[MapGroup("authors")]
[MapPost("", "register-author")]
[Description("Registers a new author in the system.")]
[MapIdResultValue]
public partial class RegisterAuthor : IValidable
{
    /// <summary>
    /// The name of the author.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The email of the author.
    /// </summary>
    public string? Email { get; set; }

    [MemberNotNullWhen(false, nameof(Name), nameof(Email))]
    public bool HasProblems([NotNullWhen(true)] out Problems? problems)
    {
        return Rules.Set<RegisterAuthor>()
            .NotEmpty(Name)
            .NotEmpty(Email)
            .Email(Email)
            .HasProblems(out problems);
    }

    [Command, WithValidateModel, ProduceNewEntity, WithUnitOfWork<IWorkContext>]
    internal Author Create()
    {
        WasValidated();
        return new Author(Name, Email);
    }
}
