using System;
using System.Reflection;

namespace CleanArchitecture.Mediator.Configuration
{

    /// <summary>
    /// Contains behaviour to configure the pipelines.
    /// </summary>
    public static class CleanArchitectureMediator
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Configures the Clean Architecture Mediator pipelines.
        /// </summary>
        /// <param name="configurationAction">The action to configure the pipelines.</param>
        /// <param name="registerSingletonServiceAction">The action to register a singleton service.</param>
        /// <param name="registerSingletonFactoryAction">The action to register a singleton factory.</param>
        /// <param name="assembliesContainingServiceImplementations">The assemblies to scan for implementations of pipe services.</param>
        public static void Configure(
            Action<PackageConfigurationBuilder> configurationAction,
            Action<Type, Type> registerSingletonServiceAction,
            Action<Type, Func<ServiceFactory, object>> registerSingletonFactoryAction,
            params Assembly[] assembliesContainingServiceImplementations)
        {
            if (configurationAction is null) throw new ArgumentNullException(nameof(configurationAction));
            if (registerSingletonServiceAction is null) throw new ArgumentNullException(nameof(registerSingletonServiceAction));
            if (registerSingletonFactoryAction is null) throw new ArgumentNullException(nameof(registerSingletonFactoryAction));
            if (assembliesContainingServiceImplementations is null) throw new ArgumentNullException(nameof(assembliesContainingServiceImplementations));

            var _PackageConfigurationBuilder = new PackageConfigurationBuilder(registerSingletonFactoryAction, registerSingletonServiceAction);
            configurationAction(_PackageConfigurationBuilder);
            _PackageConfigurationBuilder.ScanAssembliesForServiceImplementations(assembliesContainingServiceImplementations);
        }

        #endregion Methods

    }

}
