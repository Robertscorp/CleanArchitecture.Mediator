namespace CleanArchitecture.Services.DependencyInjection
{

    public class PipeAndServiceOptions
    {

        #region - - - - - - Fields - - - - - -

        private readonly ElementOptions m_PipeOptions;
        private readonly PipeServiceOptions m_PipeServiceOptions;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public PipeAndServiceOptions(ElementOptions pipeOptions, PipeServiceOptions pipeServiceOptions)
        {
            this.m_PipeOptions = pipeOptions;
            this.m_PipeServiceOptions = pipeServiceOptions;
        }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Registers a service as being required by the pipe.
        /// </summary>
        /// <typeparam name="TPipeService">The type of service required by the pipe.</typeparam>
        /// <returns>PipeAndServiceOptions.</returns>
        public PipeAndServiceOptions AddPipeService<TPipeService>()
            => this.AddPipeService(typeof(TPipeService));

        /// <summary>
        /// Registers a service as being required by the pipe.
        /// </summary>
        /// <param name="pipeService">The type of service required by the pipe.</param>
        /// <returns>PipeAndServiceOptions.</returns>
        public PipeAndServiceOptions AddPipeService(Type pipeService)
            => this.m_PipeOptions.AddPipeService(pipeService);

        /// <summary>
        /// Registers the Output Port for the pipe. Used for Validation.
        /// </summary>
        /// <typeparam name="TPipeOutputPort">The type of the Pipe's Output Port.</typeparam>
        /// <returns>ElementOptions.</returns>
        public ElementOptions WithPipeOutputPort<TPipeOutputPort>()
            => this.WithPipeOutputPort(typeof(TPipeOutputPort));

        /// <summary>
        /// Registers the Output Port for the pipe. Used for Validation.
        /// </summary>
        /// <param name="pipeOutputPort">The type of the Pipe's Output Port.</param>
        /// <returns>ElementOptions.</returns>
        public ElementOptions WithPipeOutputPort(Type pipeOutputPort)
            => this.m_PipeOptions.WithPipeOutputPort(pipeOutputPort);

        /// <summary>
        /// Provides a mechanism to get the service for a specific Use Case. Used for Validation.
        /// </summary>
        /// <param name="useCaseServiceResolver">
        /// A function to get the Use Case Service from the Use Case Input Port, Use Case Output Port, and Pipe Service.
        /// </param>
        /// <returns>ElementOptions.</returns>
        /// <remarks>If not specified, the validation process will assume there is only a single implementation for the service.</remarks>
        public ElementOptions WithUseCaseServiceResolver(Func<Type, Type, Type, Type> useCaseServiceResolver)
        {
            this.m_PipeServiceOptions.WithUseCaseServiceResolver(useCaseServiceResolver);
            return this.m_PipeOptions;
        }

        #endregion Methods

    }

}
