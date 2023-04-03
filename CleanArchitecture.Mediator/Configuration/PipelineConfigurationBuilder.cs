using CleanArchitecture.Mediator.Pipeline;
using System;

namespace CleanArchitecture.Mediator.Configuration
{

    /// <summary>
    /// A builder used to configure a pipeline.
    /// </summary>
    public class PipelineConfigurationBuilder
    {

        #region - - - - - - Constructors - - - - - -

        internal PipelineConfigurationBuilder(Type pipelineType)
            => this.PipelineConfiguration = new PipelineConfiguration(pipelineType);

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        internal PipelineConfiguration PipelineConfiguration { get; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Adds a Pipe to the Pipeline.
        /// </summary>
        /// <typeparam name="TPipe">The type of Pipe to add to the Pipeline.</typeparam>
        /// <returns>Itself.</returns>
        public PipelineConfigurationBuilder AddPipe<TPipe>() where TPipe : IPipe
        {
            this.PipelineConfiguration.PipeTypes.Add(typeof(TPipe));

            return this;
        }

        #endregion Methods

    }

}
