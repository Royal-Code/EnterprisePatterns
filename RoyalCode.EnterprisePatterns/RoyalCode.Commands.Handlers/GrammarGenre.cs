using System.Runtime.CompilerServices;

namespace RoyalCode.Commands.Handlers;

/// <summary>
/// <para>
///     This class is responsible for producing the grammar genre of a type.
/// </para>
/// </summary>
public static class GrammarGenre
{
    /// <summary>
    ///    The function that produces the grammar genre of a type.
    /// </summary>
    public static Func<Type, string> ProduceGrammarGenre { get; set; } = InternalProduceGrammarGenre;

    /// <summary>
    ///    Get the grammar genre of a type.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <returns>The grammar genre of the type.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Get<T>() => ProduceGrammarGenre(typeof(T));

    /// <summary>
    /// Default english grammar genre.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The grammar genre of the type.</returns>
    private static string InternalProduceGrammarGenre(Type type) => "The";
}
