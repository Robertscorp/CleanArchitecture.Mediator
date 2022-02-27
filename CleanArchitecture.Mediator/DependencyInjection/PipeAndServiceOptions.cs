using System;

namespace CleanArchitecture.Mediator.DependencyInjection
{

    /// <summary>
    /// The options used to configure the Use Case Pipe or the Services for an existing Pipe.
    /// </summary>
    public class PipeAndServiceOptions
    {

        #region - - - - - - Fields - - - - - -

        private readonly PipeOptions m_PipeOptions;
        private readonly PipeServiceOptions m_PipeServiceOptions;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        internal PipeAndServiceOptions(PipeOptions pipeOptions, PipeServiceOptions pipeServiceOptions)
        {
            this.m_PipeOptions = pipeOptions ?? throw new ArgumentNullException(nameof(pipeOptions));
            this.m_PipeServiceOptions = pipeServiceOptions ?? throw new ArgumentNullException(nameof(pipeServiceOptions));
        }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Registers a service as being required by the Pipe.
        /// </summary>
        /// <typeparam name="TPipeService">The type of service required by the Pipe.</typeparam>
        /// <returns>PipeAndServiceOptions.</returns>
        public PipeAndServiceOptions AddPipeService<TPipeService>()
            => this.AddPipeService(typeof(TPipeService));

        /// <summary>
        /// Registers a service as being required by the Pipe.
        /// </summary>
        /// <param name="pipeService">The type of service required by the Pipe.</param>
        /// <returns>PipeAndServiceOptions.</returns>
        public PipeAndServiceOptions AddPipeService(Type pipeService)
            => this.m_PipeOptions.AddPipeService(pipeService);

        /// <summary>
        /// Registers the Output Port for the Pipe. Used for Validation.
        /// </summary>
        /// <typeparam name="TPipeOutputPort">The type of the Pipe's Output Port.</typeparam>
        /// <returns>PipeOptions.</returns>
        public PipeOptions WithPipeOutputPort<TPipeOutputPort>()
            => this.WithPipeOutputPort(typeof(TPipeOutputPort));

        /// <summary>
        /// Registers the Output Port for the Pipe. Used for Validation.
        /// </summary>
        /// <param name="pipeOutputPort">The type of the Pipe's Output Port.</param>
        /// <returns>PipeOptions.</returns>
        public PipeOptions WithPipeOutputPort(Type pipeOutputPort)
            => this.m_PipeOptions.WithPipeOutputPort(pipeOutputPort);

        /// <summary>
        /// Provides a mechanism to get the service for a specific Use Case. Used for Validation.
        /// </summary>
        /// <param name="useCaseServiceResolver">
        /// A function to get the Use Case Service from the Use Case Input Port, Use Case Output Port, and Pipe Service.
        /// </param>
        /// <returns>PipeOptions.</returns>
        /// <remarks>If not specified, the validation process will assume there is only a single implementation for the service.</remarks>
        public PipeOptions WithUseCaseServiceResolver(Func<Type, Type, Type, Type> useCaseServiceResolver)
        {
            this.m_PipeServiceOptions.WithUseCaseServiceResolver(useCaseServiceResolver);
            return this.m_PipeOptions;
        }

        #endregion Methods

    }

}
