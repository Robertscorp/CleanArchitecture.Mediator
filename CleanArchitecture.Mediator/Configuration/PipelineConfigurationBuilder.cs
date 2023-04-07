using CleanArchitecture.Mediator.Infrastructure;
using CleanArchitecture.Mediator.Pipeline;
using System;
using System.Threading;
using System.Threading.Tasks;

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
            this.PipelineConfiguration.PipeFactories.Add(pipesByType => pipesByType.TryGetValue(typeof(TPipe), out var _Pipe) ? _Pipe : null);

            return this;
        }

        /// <summary>
        /// Adds inline behaviour to the Pipeline.
        /// </summary>
        /// <param name="inlineBehaviourAsync">The behaviour of the Pipe. Return true if the pipeline should continue. </param>
        /// <returns>Itself.</returns>
        public PipelineConfigurationBuilder AddPipe(
            Func<(object InputPort, object OutputPort, ServiceFactory ServiceFactory, NextPipeHandle NextPipeHandle, CancellationToken CancellationToken), Task> inlineBehaviourAsync)
        {
            if (inlineBehaviourAsync is null) throw new ArgumentNullException(nameof(inlineBehaviourAsync));

            this.PipelineConfiguration.PipeFactories.Add(pipesByType => new InlinePipe(inlineBehaviourAsync));

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
