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
        /// Configures the pipelines and returns the package configuration service.
        /// </summary>
        /// <param name="configurationAction">The action to configure the pipelines.</param>
        /// <returns>The package configuration, which should be registered as a singleton.</returns>
        public static object Configure(Action<PackageConfigurationBuilder> configurationAction)
        {
            var _ConfigurationBuilder = new PackageConfigurationBuilder();
            configurationAction(_ConfigurationBuilder);
            return _ConfigurationBuilder.PackageConfiguration;
        }

        #endregion Methods

    }

}
