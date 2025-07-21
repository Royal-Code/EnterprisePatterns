namespace RoyalCode.WorkContext;

/// <summary>
/// Used for components that work with service providers in their infrastructure and can share the services they provide.
/// </summary>
public interface IInfrastructureProvidesServices
{
    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    /// <param name="serviceType">An object that specifies the type of service object to get.</param>
    /// <returns>
    ///     A service object of type <paramref name="serviceType" />.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     If the service provider is not provided and it was not found in the database context.
    /// </exception>
    object GetService(Type serviceType);
    
    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of service object to get.</typeparam>
    /// <returns>
    ///     A service object of type <typeparamref name="T"></typeparamref>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     If the service provider is not provided and it was not found in the database context.
    /// </exception>
    T GetService<T>();
}
