using CleanArchitecture.Mediator.Pipes;
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
        /// Adds authentication to the pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        public PipelineConfigurationBuilder AddAuthentication()
            => this.AddPipe<AuthenticationPipe>();

        /// <summary>
        /// Adds authorisation to the pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        public PipelineConfigurationBuilder AddAuthorisation()
            => this.AddPipe<AuthorisationPipe>();

        /// <summary>
        /// Adds interactor invocation to the pipeline.
        /// </summary>
        public void AddInteractorInvocation()
            => this.AddPipe<InteractorInvocationPipe>();

        /// <summary>
        /// Adds a pipe to the pipeline.
        /// </summary>
        /// <typeparam name="TPipe">The type of pipe to add to the pipeline.</typeparam>
        /// <returns>Itself.</returns>
        public PipelineConfigurationBuilder AddPipe<TPipe>() where TPipe : IPipe
        {
            this.PipelineConfiguration.PipeFactories.Add(pipesByType => pipesByType.TryGetValue(typeof(TPipe), out var _Pipe) ? _Pipe : null);

            return this;
        }

        /// <summary>
        /// Adds inline behaviour to the pipeline.
        /// </summary>
        /// <param name="inlineBehaviourAsync">The behaviour of the pipe.</param>
        /// <returns>Itself.</returns>
        public PipelineConfigurationBuilder AddPipe(
            Func<object, object, ServiceFactory, NextPipeHandleAsync, CancellationToken, Task> inlineBehaviourAsync)
        {
            if (inlineBehaviourAsync is null) throw new ArgumentNullException(nameof(inlineBehaviourAsync));

            this.PipelineConfiguration.PipeFactories.Add(pipesByType => new InlinePipe(inlineBehaviourAsync));

            return this;
        }

        /// <summary>
        /// Adds validation to the pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        public PipelineConfigurationBuilder AddValidation()
            => this.AddPipe<ValidationPipe>();

        #endregion Methods

    }

}
