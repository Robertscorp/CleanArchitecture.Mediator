using CleanArchitecture.Mediator.Infrastructure;
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
        /// Adds Authentication to the Pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        public PipelineConfigurationBuilder AddAuthentication()
            => this.AddPipe<AuthenticationPipe>();

        /// <summary>
        /// Adds Authorisation to the Pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        public PipelineConfigurationBuilder AddAuthorisation<TAuthorisationResult>() where TAuthorisationResult : IAuthorisationResult
            => this.AddPipe<AuthorisationPipe<TAuthorisationResult>>();

        /// <summary>
        /// Adds Interactor Invocation to the Pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        public void AddInteractorInvocation()
            => this.AddPipe<InteractorInvocationPipe>();

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

        /// <summary>
        /// Adds Validation to the Pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        public PipelineConfigurationBuilder AddValidation<TValidationResult>() where TValidationResult : IValidationResult
            => this.AddPipe<ValidationPipe<TValidationResult>>();

        #endregion Methods

    }

}
