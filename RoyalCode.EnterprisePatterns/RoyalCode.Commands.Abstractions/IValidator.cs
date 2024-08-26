using RoyalCode.SmartProblems;

namespace RoyalCode.Commands.Abstractions;

/// <summary>
/// A component to validate models.
/// </summary>
/// <typeparam name="TModel">The model type to be validated</typeparam>
public interface IValidator<in TModel>
{
    /// <summary>
    /// Validates the model.
    /// </summary>
    /// <param name="model">The model to be validated.</param>
    /// <returns>
    ///     A operation result with the problems messages if the model is invalid,
    ///     or a operation result with success if the model is valid.
    /// </returns>
    Result Validate(TModel model);
}