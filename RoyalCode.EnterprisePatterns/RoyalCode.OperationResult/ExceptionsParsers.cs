
using System.Diagnostics.CodeAnalysis;

namespace RoyalCode.OperationResult;

/// <summary>
/// <para>
///     Static class to manage custom exceptions parsers.
/// </para>
/// <para>
///     The parsers added here will be used by the <see cref="ResultMessage"/> class, 
///     when creating a message from an exception.
/// </para>
/// </summary>
public static class ExceptionsParsers
{
    private static List<IExceptionParser>? parsers;

    /// <summary>
    /// <para>
    ///     Add a parser to the list of parsers.
    /// </para>
    /// </summary>
    /// <param name="parser">A exception parser.</param>
    public static void AddParser(IExceptionParser parser)
    {
        parsers ??= new List<IExceptionParser>();
        parsers.Add(parser);
    }

    /// <summary>
    /// <para>
    ///     Remove a parser from the list of parsers.
    /// </para>
    /// </summary>
    /// <param name="parser"></param>
    public static void RemoveParser(IExceptionParser parser)
    {
        parsers?.Remove(parser);
    }

    /// <summary>
    /// <para>
    ///     Try to parse the exception to a result message.
    /// </para>
    /// </summary>
    /// <param name="ex">The exception occured into a operation message.</param>
    /// <param name="message">The output message.</param>
    /// <returns>True if the exception was parsed.</returns>
    public static bool TryParse(Exception ex, [NotNullWhen(true)] out ResultMessage? message)
    {
        if (parsers is not null)
            foreach (var parser in parsers)
                if (parser.TryParse(ex, out message))
                    return true;

        message = null;
        return false;
    }
}

/// <summary>
/// <para>
///     A parser of exceptions to result messages.
/// </para>
/// <para>
///     Add a parser to <see cref="ExceptionsParsers"/> to use it.
/// </para>
/// </summary>
public interface IExceptionParser
{
    /// <summary>
    /// <para>
    ///     Try to parse the exception to a result message.
    /// </para>
    /// </summary>
    /// <param name="ex">The exception occured into a operation message.</param>
    /// <param name="message">The output message.</param>
    /// <returns>True if the exception was parsed.</returns>
    bool TryParse(Exception ex, [NotNullWhen(true)] out ResultMessage? message);
}