
using RoyalCode.OperationResult;

namespace RoyalCode.Commands.Abstractions;

/// <summary>
/// Used with <see cref="ICommandContext{TModel}"/> or <see cref="ICommandContext{TRootEntity, TModel}"/>
/// when the context values must be validated for the <see cref="IContextBuilder{TContext, TModel}"/>.
/// </summary>
public interface IValidableContext
{
    /// <summary>
    /// Validate the context values.
    /// </summary>
    /// <returns>
    ///     A operation result with the problems messages if the context is invalid,
    ///     or a operation result with success if the context is valid.
    /// </returns>
    IOperationResult Validate();
}