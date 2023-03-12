using System.Reflection;

namespace RoyalCode.Persistence.Searches.Abstractions.Linq.Selector.Converters;

/// <summary>
/// A component that solves the mapping between properties of two types to create a select expression 
/// of <see cref="IQueryable{T}"/>.
/// </summary>
public interface ISelectResolver
{
    /// <summary>
    /// Get the resolutions for the mapping between properties of two types.
    /// </summary>
    /// <param name="entityType">The type of the entity, the source of the select expression.</param>
    /// <param name="dtoType">The type of the DTO, the target of the select expression.</param>
    /// <param name="ctor">The constructor of the DTO.</param>
    /// <returns>
    ///     A list of resolutions for the mapping between properties of two types,
    ///     or null if the mapping cannot be resolved.
    /// </returns>
    IEnumerable<SelectResolution>? GetResolutions(Type entityType, Type dtoType, out ConstructorInfo? ctor);
}

