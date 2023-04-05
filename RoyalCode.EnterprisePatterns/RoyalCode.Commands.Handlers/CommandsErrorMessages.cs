using System.Runtime.CompilerServices;

namespace RoyalCode.Commands.Handlers;

/// <summary>
/// <para>
///     A static class with error messages for commands.
/// </para>
/// </summary>
public static class CommandsErrorMessages
{
    /// <summary>
    /// The pattern for the error message when a command is not found.
    /// </summary>
    public const string NotFoundPattern = "{0} {1} was not found for id: {2}";

    /// <summary>
    /// Utility to use extensions methods to set the error messages of commands in a fluent way.
    /// </summary>
    public static Lang Language => new();

    /// <summary>
    /// An internal class to use extension methods for the <see cref="CommandsErrorMessages"/> class.
    /// </summary>
    public class Lang { internal Lang() { } }

    /// <summary>
    /// Create a message for a command not found.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="id">The identifier of the entity.</param>
    /// <returns>A message for entity not found.</returns> 
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string CreateNotFoundMessage<TEntity>(object? id)
    {
        return string.Format(
            NotFoundPattern,
            GrammarGenre.Get<TEntity>(),
            DisplayNames.Get<TEntity>(),
            id);
    }
}
