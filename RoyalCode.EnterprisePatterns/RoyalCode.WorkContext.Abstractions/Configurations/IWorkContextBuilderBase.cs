using RoyalCode.UnitOfWork.Configurations;

namespace RoyalCode.WorkContext.Configurations;

/// <summary>
/// <para>
///     Interface to configure a work context, which is an extension of the unit of work pattern.
/// </para>
/// <para>
///     This builder is designed to be used with dependency injection and provides configuration
///     for repositories and search services within the work context.
/// </para>
/// <para>
///     Inherits all configuration capabilities from <see cref="IUnitOfWorkBuilderBase{TBuilder}"/>.
/// </para>
/// </summary>
/// <typeparam name="TBuilder">
///     The type of the builder implementing this interface, enabling fluent configuration.
/// </typeparam>
public interface IWorkContextBuilderBase<out TBuilder> : IUnitOfWorkBuilderBase<TBuilder>
    where TBuilder : IWorkContextBuilderBase<TBuilder>
{ }
