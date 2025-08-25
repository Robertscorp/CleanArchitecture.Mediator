using CleanArchitecture.Mediator.Internal;
using System;

namespace CleanArchitecture.Mediator.Setup
{

    /// <summary>
    /// Represents configuring the CleanArchitecture.Mediator package.
    /// </summary>
    public class PackageConfiguration
    {

        #region - - - - - - Fields - - - - - -

        private readonly PackageRegistration m_PackageRegistration = new PackageRegistration();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        internal PackageConfiguration(PackageRegistration packageRegistration)
            => this.m_PackageRegistration = packageRegistration;

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Adds and configures a pipeline.
        /// </summary>
        /// <typeparam name="TPipeline">The type of pipeline to add.</typeparam>
        /// <param name="configurationAction">An action used to configure the pipeline. Cannot be null.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="configurationAction"/> is null.</exception>
        public PackageConfiguration AddPipeline<TPipeline>(Action<PipelineConfiguration<TPipeline>> configurationAction) where TPipeline : Pipeline
        {
            if (configurationAction is null) throw new ArgumentNullException(nameof(configurationAction));

            var _PipelineBuilder = new PipelineConfiguration<TPipeline>(this.m_PackageRegistration);

            configurationAction(_PipelineBuilder);

            _ = this.m_PackageRegistration
                    .AddSingletonInstance(typeof(PipelineHandleAccessor<TPipeline>), _PipelineBuilder.GetPipelineHandleAccessor())
                    .AddSingletonServiceImplementation(typeof(TPipeline), typeof(TPipeline));

            return this;
        }

        #endregion Methods

    }

}
