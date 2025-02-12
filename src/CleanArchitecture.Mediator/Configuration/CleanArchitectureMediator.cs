using System;

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
        public static void Configure(
            Action<PackageConfigurationBuilder> configurationAction,
            Action<Type> registerSingletonServiceAction,
            Action<Type, Func<ServiceFactory, object>> registerSingletonFactoryAction)
        {
            if (configurationAction is null) throw new ArgumentNullException(nameof(configurationAction));
            if (registerSingletonServiceAction is null) throw new ArgumentNullException(nameof(registerSingletonServiceAction));
            if (registerSingletonFactoryAction is null) throw new ArgumentNullException(nameof(registerSingletonFactoryAction));

            configurationAction(new PackageConfigurationBuilder(registerSingletonFactoryAction, service => registerSingletonServiceAction(service)));
        }

        #endregion Methods

    }

}
