using CleanArchitecture.Mediator.Internal;
using System;

namespace CleanArchitecture.Mediator.Configuration
{

    /// <summary>
    /// A builder used to configure the CleanArchitecture.Mediator package.
    /// </summary>
    public class PackageConfigurationBuilder
    {

        #region - - - - - - Fields - - - - - -

        private readonly Action<Type, Func<ServiceFactory, object>> m_OnSingletonFactoryAdded;
        private readonly Action<Type> m_OnSingletonServiceAdded;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        internal PackageConfigurationBuilder(Action<Type, Func<ServiceFactory, object>> onSingletonFactoryAdded, Action<Type> onSingletonServiceAdded)
        {
            this.m_OnSingletonFactoryAdded = onSingletonFactoryAdded ?? throw new ArgumentNullException(nameof(onSingletonFactoryAdded));
            this.m_OnSingletonServiceAdded = onSingletonServiceAdded ?? throw new ArgumentNullException(nameof(onSingletonServiceAdded));
        }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Adds and configures a pipeline.
        /// </summary>
        /// <typeparam name="TPipeline">The type of pipeline to add.</typeparam>
        /// <param name="configurationAction">An action used to configure the pipeline.</param>
        /// <returns>Itself.</returns>
        public PackageConfigurationBuilder AddPipeline<TPipeline>(Action<PipelineBuilder<TPipeline>> configurationAction) where TPipeline : Pipeline
        {
            var _PipelineBuilder = new PipelineBuilder<TPipeline>(this.m_OnSingletonServiceAdded);

            configurationAction(_PipelineBuilder);

            this.m_OnSingletonFactoryAdded(typeof(PipelineHandleAccessor<TPipeline>), _PipelineBuilder.GetPipelineHandleAccessorFactory());
            this.m_OnSingletonServiceAdded(typeof(TPipeline));

            return this;
        }

        #endregion Methods

    }

}
