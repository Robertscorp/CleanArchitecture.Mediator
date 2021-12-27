namespace CleanArchitecture.Services.DependencyInjection
{

    public class ElementOptions
    {

        #region - - - - - - Constructors - - - - - -

        public ElementOptions(Type elementType)
            => this.ElementType = elementType;

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        internal Type ElementType { get; }

        internal Type? PipeOutputPort { get; private set; }

        internal List<PipeServiceOptions> PipeServiceOptions { get; } = new();

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Registers the Output Port for the pipe. Used for Validation.
        /// </summary>
        /// <typeparam name="TPipeOutputPort">The type of the Pipe's Output Port.</typeparam>
        /// <returns>Itself.</returns>
        public ElementOptions WithPipeOutputPort<TPipeOutputPort>()
            => this.WithPipeOutputPort(typeof(TPipeOutputPort));

        /// <summary>
        /// Registers the Output Port for the pipe. Used for Validation.
        /// </summary>
        /// <param name="pipeOutputPort">The type of the Pipe's Output Port.</param>
        /// <returns>Itself.</returns>
        public ElementOptions WithPipeOutputPort(Type pipeOutputPort)
        {
            this.PipeOutputPort = pipeOutputPort;
            return this;
        }

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
        {
            var _PipeServiceOptions = new PipeServiceOptions(pipeService);
            this.PipeServiceOptions.Add(_PipeServiceOptions);
            return new PipeAndServiceOptions(this, _PipeServiceOptions);
        }

        #endregion Methods

    }

}
