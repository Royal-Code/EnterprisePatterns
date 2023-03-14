#pragma warning disable S3925 // "ISerializable" should be implemented correctly

namespace RoyalCode.Repositories.Abstractions;

/// <summary>
/// <para>
///     Exception throwed when the repository for the entity type is not found.
/// </para>
/// </summary>
public sealed class RepositoryNotFoundException : InvalidOperationException
{
    /// <summary>
    /// <para>
    ///     Create a new instance of <see cref="RepositoryNotFoundException"/>.
    /// </para>
    /// </summary>
    /// <param name="message">The message of the exception.</param>
    public RepositoryNotFoundException(string message) : base(message) { }
}