using System;
using System.Collections.Generic;

namespace CleanArchitecture.Mediator.DependencyInjection
{

    /// <summary>
    /// The options used to configure the Use Case Pipe.
    /// </summary>
    public class PipeOptions
    {

        #region - - - - - - Constructors - - - - - -

        internal PipeOptions(Type pipeType)
            => this.PipeType = pipeType ?? throw new ArgumentNullException(nameof(pipeType));

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        internal Type PipeOutputPort { get; private set; }

        internal List<PipeServiceOptions> PipeServiceOptions { get; } = new List<PipeServiceOptions>();

        internal Type PipeType { get; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Registers the Output Port for the Pipe. Used for Validation.
        /// </summary>
        /// <typeparam name="TPipeOutputPort">The type of the Pipe's Output Port.</typeparam>
        /// <returns>Itself.</returns>
        public PipeOptions WithPipeOutputPort<TPipeOutputPort>()
            => this.WithPipeOutputPort(typeof(TPipeOutputPort));

        /// <summary>
        /// Registers the Output Port for the Pipe. Used for Validation.
        /// </summary>
        /// <param name="pipeOutputPort">The type of the Pipe's Output Port.</param>
        /// <returns>Itself.</returns>
        public PipeOptions WithPipeOutputPort(Type pipeOutputPort)
        {
            this.PipeOutputPort = pipeOutputPort;
            return this;
        }

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
        {
            var _PipeServiceOptions = new PipeServiceOptions(pipeService);
            this.PipeServiceOptions.Add(_PipeServiceOptions);
            return new PipeAndServiceOptions(this, _PipeServiceOptions);
        }

        #endregion Methods

    }

}
