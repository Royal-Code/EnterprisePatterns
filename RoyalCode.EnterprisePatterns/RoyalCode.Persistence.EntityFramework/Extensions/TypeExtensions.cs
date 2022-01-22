
namespace RoyalCode.Persistence.EntityFramework.Extensions;

/// <summary>
/// <para>
///     Internal extensions methods.
/// </para>
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// <para>
    ///     Gets the <see cref="Type.FullName"/> concatenated with the assembly name.
    /// </para>
    /// <para>
    ///     Similar to <see cref="Type.AssemblyQualifiedName"/>, but without the assembly attributes like version or hash.
    /// </para>
    /// </summary>
    /// <param name="type">Type to be obtained the full name.</param>
    /// <returns>The fullname.</returns>
    public static string FullNameWithAssembly(this Type type)
    {
        var names = type.AssemblyQualifiedName!.Split(',');
        return names[0] + ',' + names[1];
    }
}
