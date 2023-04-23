using CleanArchitecture.Mediator.Internal;
using System;

namespace CleanArchitecture.Mediator.Configuration
{

    /// <summary>
    /// A builder used to configure the CleanArchitecture.Mediator package.
    /// </summary>
    public class PackageConfigurationBuilder
    {

        #region - - - - - - Properties - - - - - -

        internal PackageConfiguration PackageConfiguration { get; } = new PackageConfiguration();

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Adds and configures a pipeline.
        /// </summary>
        /// <typeparam name="TPipeline">The type of pipeline to add.</typeparam>
        /// <param name="configurationAction">An action used to configure the pipeline.</param>
        /// <returns>Itself.</returns>
        public PackageConfigurationBuilder AddPipeline<TPipeline>(Action<PipelineConfigurationBuilder> configurationAction)
            where TPipeline : IPipeline
        {
            var _ConfigurationBuilder = new PipelineConfigurationBuilder(typeof(TPipeline));

            configurationAction(_ConfigurationBuilder);

            this.PackageConfiguration.PipelineConfigurations.Add(_ConfigurationBuilder.PipelineConfiguration);

            return this;
        }

        #endregion Methods

    }

}
