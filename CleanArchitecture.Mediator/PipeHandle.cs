using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// A handle to a pipe in the pipeline.
    /// </summary>
    public class PipeHandle
    {

        #region - - - - - - Fields - - - - - -

        private readonly IPipe m_Pipe;
        private readonly PipeHandle m_NextPipeHandle;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        internal PipeHandle(IPipe pipe, PipeHandle nextPipeHandle)
        {
            this.m_Pipe = pipe;
            this.m_NextPipeHandle = nextPipeHandle;
        }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Invokes the pipe with the specified parameters.
        /// </summary>
        /// <typeparam name="TInputPort">The type of input port.</typeparam>
        /// <typeparam name="TOutputPort">The type of output port.</typeparam>
        /// <param name="inputPort">The input to the pipeline.</param>
        /// <param name="outputPort">The output mechanism for the pipeline.</param>
        /// <param name="serviceFactory">The factory used to get the service object of the specified type.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        public Task InvokePipeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            CancellationToken cancellationToken) where TInputPort : IInputPort<TOutputPort>
            => this.m_Pipe?.InvokeAsync(inputPort, outputPort, serviceFactory, this.m_NextPipeHandle, cancellationToken)
                ?? Task.CompletedTask;

        #endregion Methods

    }

}
