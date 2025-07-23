using System;

namespace CleanArchitecture.Mediator.Setup
{

    /// <summary>
    /// Contains behaviour to configure and register the Clean Architecture Mediator package.
    /// </summary>
    public static class CleanArchitectureMediator
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Configures and registers the Clean Architecture Mediator package.
        /// </summary>
        /// <param name="configurationAction">The action to configure pipelines. Cannot be null.</param>
        /// <param name="registrationAction">The action to configure how to register the package in the service container. Cannot be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="configurationAction"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="registrationAction"/> is null.</exception>
        public static void Setup(Action<PackageConfiguration> configurationAction, Action<PackageRegistration> registrationAction)
        {
            if (configurationAction is null) throw new ArgumentNullException(nameof(configurationAction));
            if (registrationAction is null) throw new ArgumentNullException(nameof(registrationAction));

            var _PackageRegistration = new PackageRegistration();
            registrationAction(_PackageRegistration);
            configurationAction(new PackageConfiguration(_PackageRegistration));
            _PackageRegistration.ScanForImplementations();
        }

        #endregion Methods

    }

}
