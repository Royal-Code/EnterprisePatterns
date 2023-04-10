
using RoyalCode.OperationResult;

namespace RoyalCode.Commands.Abstractions;

/// <summary>
/// <para>
///     When a command is executed from a context, the context can be validated before the command is executed.
/// </para>
/// <para>
///     To validate the context before command execution, the context must implement this interface.
/// </para>
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