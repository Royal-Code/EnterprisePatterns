using RoyalCode.Repositories.Abstractions;
using RoyalCode.Searches.Abstractions;
using RoyalCode.UnitOfWork.Abstractions;

namespace RoyalCode.WorkContext.Abstractions;

/// <summary>
/// <para>
///     A <see cref="IWorkContext"/> is an extension of the <see cref="IUnitOfWork"/> 
///     that enables the access to data access components related to a context,
///     or one could say, related to a persistence unit.
/// </para>
/// <para>
///     By default, the <see cref="IWorkContext"/> is used to access the repositories and search components.
/// </para>
/// <para>
///     For extended features, it is possible to create a new interface that inherits from <see cref="IWorkContext"/>
///     and add new methods to access the new components.
/// </para>
/// <para>
///     This is design to encapsulate the common required services for the application services.
///     It is a mistake compare this component with a service locator, because the <see cref="IWorkContext"/> 
///     is not a service locator, neither is a dependency injection container,
///     and the services that are provided is part of the persistence unit.
/// </para>
/// </summary>
public interface IWorkContext : IUnitOfWork, IEntityManager, ISearchable { }