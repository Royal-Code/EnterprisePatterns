using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using RoyalCode.UnitOfWork.Configurations;
using System.Reflection;

namespace RoyalCode.UnitOfWork.EntityFramework.Configurations;

/// <summary>
/// <para>
///     Interface to configure one unit of work with the DbContext.
/// </para>
/// <para>
///     It is designed to work with dependency injection.
/// </para>
/// </summary>
/// <typeparam name="TDbContext">The type of DbContext for the unit of work.</typeparam>
public interface IUnitOfWorkBuilder<out TDbContext> 
    : IUnitOfWorkBuilder<TDbContext, IUnitOfWorkBuilder<TDbContext>>
    where TDbContext : DbContext
{ }

/// <summary>
/// <para>
///     Interface to configure one unit of work with the DbContext.
/// </para>
/// <para>
///     It is designed to work with dependency injection.
/// </para>
/// </summary>
/// <typeparam name="TDbContext">The type of DbContext for the unit of work.</typeparam>
/// <typeparam name="TBuilder"> The type of the builder for the unit of work.</typeparam>
public interface IUnitOfWorkBuilder<out TDbContext, out TBuilder> : IUnitOfWorkBuilderBase<TBuilder>
    where TDbContext : DbContext
    where TBuilder : IUnitOfWorkBuilder<TDbContext, TBuilder>
{
    /// <summary>
    /// <para>
    ///     Configure the <see cref="DbContext"/> for the unit of work.
    /// </para>
    /// <para>
    ///     The configuration is done by the <see cref="IConfigureOptions{TDbContext}"/>
    ///     registered in the services.
    /// </para>
    /// <para>
    ///     When the <see cref="IConfigureOptions{TDbContext}"/> is not registered, an
    ///     <see cref="InvalidOperationException"/> is thrown.
    /// </para>
    /// </summary>
    /// <remarks>
    ///     This method call AddDbContext with the <see cref="ServiceLifetime"/> of the unit of work.
    /// </remarks>
    /// <returns>The same instance.</returns>
    /// <exception cref="InvalidOperationException">
    ///     The <see cref="IConfigureOptions{TDbContext}"/> is not registered.
    /// </exception>
    public TBuilder ConfigureDbContextWithService()
    {
        Services.AddDbContext<TDbContext>((sp, builder) =>
        {
            var configurators = sp.GetService<IEnumerable<IConfigureOptions<TDbContext>>>();

            if (configurators is null || !configurators.Any())
                throw new InvalidOperationException(
                    "The IConfigureOptions is not registered. " +
                    "When using the ConfigureWithService method, it is necessary to register the " +
                    "IConfigureOptions<TDbContext>.");

            foreach (var configurator in configurators)
                configurator.Configure(builder);

        }, Lifetime);
        return (TBuilder)this;
    }

    /// <summary>
    /// <para>
    ///     Configure the <see cref="DbContext"/> for the unit of work.
    /// </para>
    /// <para>
    ///     The configuration is done by the <see cref="IConfigureOptions{TDbContext}"/>
    ///     registered in the services.
    /// </para>
    /// <para>
    ///     When the <see cref="IConfigureOptions{TDbContext}"/> is not registered, an
    ///     <see cref="InvalidOperationException"/> is thrown.
    /// </para>
    /// </summary>
    /// <remarks>
    ///     This method call AddDbContextPool.
    /// </remarks>
    /// <returns>The same instance.</returns>
    /// <exception cref="InvalidOperationException">
    ///     The <see cref="IConfigureOptions{TDbContext}"/> is not registered.
    /// </exception>
    public TBuilder ConfigureDbContextPoolWithService()
    {
        Services.AddDbContextPool<TDbContext>((sp, builder) =>
        {
            var configurators = sp.GetService<IEnumerable<IConfigureOptions<TDbContext>>>();

            if (configurators is null || !configurators.Any())
                throw new InvalidOperationException(
                    "The IConfigureOptions is not registered. " +
                    "When using the ConfigureWithService method, it is necessary to register the " +
                    "IConfigureOptions<TDbContext>.");

            foreach (var configurator in configurators)
                configurator.Configure(builder);

        });
        return (TBuilder)this;
    }

    /// <summary>
    /// Configure the <see cref="DbContext"/> for the unit of work as pooled.
    /// </summary>
    /// <param name="configurer">Action to configure.</param>
    /// <returns>The same instance.</returns>
    public TBuilder ConfigureDbContextPool(Action<DbContextOptionsBuilder> configurer)
    {
        ArgumentNullException.ThrowIfNull(configurer);
        Services.AddDbContextPool<TDbContext>(configurer);
        return (TBuilder)this;
    }

    /// <summary>
    /// Configure the <see cref="DbContext"/> for the unit of work as pooled.
    /// </summary>
    /// <param name="configurer">Action to configure.</param>
    /// <returns>The same instance.</returns>
    public TBuilder ConfigureDbContextPool(Action<IServiceProvider, DbContextOptionsBuilder> configurer)
    {
        ArgumentNullException.ThrowIfNull(configurer);
        Services.AddDbContextPool<TDbContext>(configurer);
        return (TBuilder)this;
    }

    /// <summary>
    /// Configure the <see cref="DbContext"/> for the unit of work..
    /// </summary>
    /// <param name="configurer">Action to configure.</param>
    /// <returns>The same instance.</returns>
    public TBuilder ConfigureDbContext(Action<DbContextOptionsBuilder>? configurer = null)
    {
        Services.AddDbContext<TDbContext>(configurer, Lifetime);
        return (TBuilder)this;
    }

    /// <summary>
    /// Configure the <see cref="DbContext"/> for the unit of work..
    /// </summary>
    /// <param name="configurer">Action to configure.</param>
    /// <returns>The same instance.</returns>
    public TBuilder ConfigureDbContext(Action<IServiceProvider, DbContextOptionsBuilder> configurer)
    {
        ArgumentNullException.ThrowIfNull(configurer);
        Services.AddDbContext<TDbContext>(configurer, Lifetime);
        return (TBuilder)this;
    }

    /// <summary>
    /// Adds an action to configure the <see cref="ModelBuilder"/> of a <see cref="DbContext"/>.
    /// </summary>
    /// <param name="configure">Action to configure.</param>
    /// <returns>The same instance.</returns>
    public TBuilder ConfigureModel(Action<ModelBuilder> configure)
    {
        InternalConfigureModel<TDbContext>.GetFromServices(Services).Configure(configure);
        return (TBuilder)this;
    }

    /// <summary>
    /// Adds an action to configure the <see cref="DbContextOptionsBuilder"/> of a <see cref="DbContext"/>.
    /// </summary>
    /// <param name="configure">Action to configure.</param>
    /// <returns>The same instance.</returns>
    public TBuilder ConfigureOptions(Action<DbContextOptionsBuilder> configure)
    {
        ConfigureOptionsAction<TDbContext> configureOptions = (sp, builder) => configure(builder);
        Services.AddSingleton(configureOptions);
        Services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<TDbContext>, InternalConfigureOptions<TDbContext>>());
        return (TBuilder)this;
    }

    /// <summary>
    /// Adds an action to configure the <see cref="DbContextOptionsBuilder"/> of a <see cref="DbContext"/>.
    /// </summary>
    /// <param name="configure">Action to configure.</param>
    /// <returns>The same instance.</returns>
    public TBuilder ConfigureOptions(Action<IServiceProvider, DbContextOptionsBuilder> configure)
    {
        ConfigureOptionsAction<TDbContext> configureOptions = (sp, builder) => configure(sp, builder);
        Services.AddSingleton(configureOptions);
        Services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<TDbContext>, InternalConfigureOptions<TDbContext>>());
        return (TBuilder)this;
    }

    /// <summary>
    /// Adds an action to configure the <see cref="ModelConfigurationBuilder"/> which can apply
    /// convention configurations for a <see cref="DbContext"/>.
    /// </summary>
    /// <param name="configure">Action to configure.</param>
    /// <returns>The same instance.</returns>
    public TBuilder ConfigureConventions(Action<ModelConfigurationBuilder> configure)
    {
        var service = new InternalConfigureConventions<TDbContext>(configure);
        Services.AddSingleton<IConfigureConventions<TDbContext>>(service);
        return (TBuilder)this;
    }

    /// <summary>
    /// Adds an action to configure the <see cref="ModelConfigurationBuilder"/> 
    /// to turns on the creation of lazy loading proxies.
    /// </summary>
    /// <returns>The same instance.</returns>
    public TBuilder UseLazyLoadingProxies()
    {
        return ConfigureOptions(static ob => ob.UseLazyLoadingProxies());
    }

    /// <summary>
    /// Adds an action to configure the <see cref="DbContextOptionsBuilder"/> to use a logger factory
    /// and when <paramref name="isDevelopment"/> is true, enables sensitive data logging and detailed errors.
    /// </summary>
    /// <param name="isDevelopment">Determines whether to enable sensitive data logging and detailed errors.</param>
    /// <returns>The same instance.</returns>
    public TBuilder UseLoggerFactoryAndEnableSensitiveDataLogging(bool isDevelopment = false)
    {
        return ConfigureOptions((sp, ob) =>
        {
            if (isDevelopment)
            {
                ob.EnableSensitiveDataLogging();
                ob.EnableDetailedErrors();
            }
            ob.UseLoggerFactory(sp.GetRequiredService<ILoggerFactory>());
        });
    }

    /// <summary>
    /// Adds an action to configure the <see cref="ModelBuilder"/> and applies all entity configurations
    /// from the specified assembly.
    /// Additionally, if <paramref name="addRepositories"/> is true, it will also add repositories
    /// for the configured entities.
    /// </summary>
    /// <param name="assembly">The assembly from which to get the entity configurations.</param>
    /// <param name="addRepositories">Determines whether to add repositories for the configured entities.</param>
    /// <returns>The same instance.</returns>
    public TBuilder ConfigureMappingsFromAssembly(Assembly assembly, bool addRepositories)
    {
        if (addRepositories)
            AddRepositories(assembly);
        return ConfigureModel(modelBuilder =>
        {
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        });
    }

    /// <summary>
    /// Adds an action to configure the <see cref="ModelBuilder"/> and applies all entity configurations
    /// from the assembly containing the specified type.
    /// Additionally, if <paramref name="addRepositories"/> is true, it will also add repositories
    /// for the configured entities.
    /// </summary>
    /// <typeparam name="TTypeFromAssembly">Type from which to get the assembly.</typeparam>
    /// <param name="addRepositories">Determines whether to add repositories for the configured entities.</param>
    /// <returns>The same instance.</returns>
    public TBuilder ConfigureMappingsFromAssembly<TTypeFromAssembly>(bool addRepositories)
        where TTypeFromAssembly : class
    {
        return ConfigureMappingsFromAssembly(typeof(TTypeFromAssembly).Assembly, addRepositories);
    }

    /// <summary>
    /// <para>
    ///     Adds repositories for all entity types found in the specified assembly.
    /// </para>
    /// <para>
    ///     Will scan the assembly for types that implement <see cref="IEntityTypeConfiguration{TEntity}"/>,
    ///     so the entity type will be the generic type argument of the interface.
    /// </para>
    /// <para>
    ///     For each entity type found, it will be added a repository related to the unit of work.
    /// </para>
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public TBuilder AddRepositories(Assembly assembly)
    {
        // gets all entities types from the assembly getting the configurations (IEntityTypeConfiguration<>)
        var entityTypes = assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract)
            .SelectMany(type => type.GetInterfaces())
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
            .Select(i => i.GetGenericArguments()[0]);

        // adds repositories for each entity type
        foreach (var type in entityTypes)
        {
            ConfigureRepositories(r => r.Add(type));
        }

        return (TBuilder)this;
    }
}

internal sealed class InternalConfigureOptions<TDb> : IConfigureOptions<TDb>
        where TDb : DbContext
{
    private readonly IServiceProvider sp;

    public InternalConfigureOptions(IServiceProvider sp)
    {
        this.sp = sp;
    }

    public void Configure(DbContextOptionsBuilder optionsBuilder)
    {
        var configurations = sp.GetRequiredService<IEnumerable<ConfigureOptionsAction<TDb>>>();
        foreach (var action in configurations)
        {
            action(sp, optionsBuilder);
        }
    }
}

internal delegate void ConfigureOptionsAction<TDb>(IServiceProvider sp, DbContextOptionsBuilder optionsBuilder)
    where TDb : DbContext;

internal sealed class InternalConfigureModel<TDb> : IConfigureModel<TDb>
    where TDb : DbContext
{
    public static InternalConfigureModel<TDb> GetFromServices(IServiceCollection services)
    {
        var descriptor = services.FirstOrDefault(d => d.ImplementationType == typeof(InternalConfigureModel<TDb>));
        if (descriptor is null || descriptor.ImplementationInstance is not InternalConfigureModel<TDb> options)
        {
            options = new InternalConfigureModel<TDb>();
            services.AddSingleton<IConfigureModel<TDb>>(options);
        }
        return options;
    }

    private Action<ModelBuilder>? configure;

    public void Configure(ModelBuilder modelBuilder)
    {
        if (configure is null)
            return;

        configure(modelBuilder);
    }

    public void Configure(Action<ModelBuilder> configure)
    {
        if (this.configure is null)
            this.configure = configure;
        else
            this.configure += configure;
    }
}

internal sealed class InternalConfigureConventions<TDb> : IConfigureConventions<TDb>
    where TDb : DbContext
{
    private readonly Action<ModelConfigurationBuilder> configure;

    public InternalConfigureConventions(Action<ModelConfigurationBuilder> configure)
    {
        this.configure = configure;
    }

    public void Configure(ModelConfigurationBuilder builder)
    {
        configure(builder);
    }
}