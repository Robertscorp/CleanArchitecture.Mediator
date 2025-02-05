using CleanArchitecture.Mediator.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Configuration
{

    /// <summary>
    /// A builder used to configure a pipeline.
    /// </summary>
    public class PipelineBuilder<TPipeline> where TPipeline : Pipeline
    {

        #region - - - - - - Fields - - - - - -

        private readonly Action<Type> m_OnPipeAdded;
        private readonly List<object> m_RegisteredPipes = new List<object>();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        internal PipelineBuilder(Action<Type> onPipeAdded)
            => this.m_OnPipeAdded = onPipeAdded ?? throw new ArgumentNullException(nameof(onPipeAdded));

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Adds authentication to the pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        public PipelineBuilder<TPipeline> AddAuthentication()
            => this.AddPipe<AuthenticationPipe>();

        /// <summary>
        /// Adds authorisation to the pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        public PipelineBuilder<TPipeline> AddAuthorisation()
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
        public PipelineBuilder<TPipeline> AddPipe<TPipe>() where TPipe : IPipe
        {
            this.m_OnPipeAdded(typeof(TPipe));
            this.m_RegisteredPipes.Add(typeof(TPipe));

            return this;
        }

        /// <summary>
        /// Adds inline behaviour to the pipeline.
        /// </summary>
        /// <param name="inlineBehaviourAsync">The behaviour of the pipe.</param>
        /// <returns>Itself.</returns>
        public PipelineBuilder<TPipeline> AddPipe(
            Func<object, object, ServiceFactory, NextPipeHandleAsync, CancellationToken, Task> inlineBehaviourAsync)
        {
            if (inlineBehaviourAsync is null) throw new ArgumentNullException(nameof(inlineBehaviourAsync));

            this.m_RegisteredPipes.Add(new InlinePipe(inlineBehaviourAsync));

            return this;
        }

        /// <summary>
        /// Adds validation to the pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        public PipelineBuilder<TPipeline> AddValidation()
            => this.AddPipe<ValidationPipe>();

        internal Func<ServiceFactory, PipelineHandleAccessor<TPipeline>> GetPipelineHandleAccessorFactory()
            => serviceFactory
                => new PipelineHandleAccessor<TPipeline>(
                    this.m_RegisteredPipes
                        .Select(p => p as IPipe ?? (IPipe)serviceFactory((Type)p))
                        .Reverse()
                        .Aggregate(
                            new PipeHandle(null, null),
                            (nextPipeHandle, pipe) => new PipeHandle(pipe, nextPipeHandle)));

        #endregion Methods

    }

}
