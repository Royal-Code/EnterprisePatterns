using RoyalCode.OperationResult;
using RoyalCode.WorkContext.Abstractions;

namespace RoyalCode.Commands.Abstractions;

/// <summary>
/// Interface for handlers that validate the command data.
/// </summary>
/// <typeparam name="TModel">The type of the command data.</typeparam>
public interface IValidationHandler<in TModel>
{
    /// <summary>
    /// Execute the validation of the command data.
    /// </summary>
    /// <param name="context">The work context.</param>
    /// <param name="model">The command data.</param>
    /// <returns>The result of the validation.</returns>
    IOperationResult Validate(IWorkContext context, TModel model);
}
