using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Pipeline
{

    /// <summary>
    /// Encapsulates the invocation of a Pipe to allow the same Pipe to be invoked by multiple Pipelines.
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
        /// <typeparam name="TUseCaseInputPort">The type of the Use Case's Input Port.</typeparam>
        /// <typeparam name="TUseCaseOutputPort">The type of the Use Case's Output Port.</typeparam>
        /// <param name="inputPort">The Use Case's Input Port.</param>
        /// <param name="outputPort">The Use Case's Output Port.</param>
        /// <param name="serviceFactory">The factory used to get the service object of the specified type.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        public Task InvokePipeAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            ServiceFactory serviceFactory,
            CancellationToken cancellationToken)
            => this.m_Pipe?.InvokeAsync(inputPort, outputPort, serviceFactory, this.m_NextPipeHandle, cancellationToken)
                ?? Task.CompletedTask;

        #endregion Methods

    }

}
