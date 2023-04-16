using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// A single Pipe in the Pipeline.
    /// </summary>
    public interface IPipe
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Handles the relevant behaviour for this section of the Pipeline.
        /// </summary>
        /// <typeparam name="TInputPort">The type of Input Port.</typeparam>
        /// <typeparam name="TOutputPort">The type of Output Port.</typeparam>
        /// <param name="inputPort">The input to the pipeline.</param>
        /// <param name="outputPort">The output mechanism for the pipeline.</param>
        /// <param name="serviceFactory">The factory used to get the service object of the specified type.</param>
        /// <param name="nextPipeHandle">The handle to the next Pipe in the Pipeline.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        Task InvokeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            PipeHandle nextPipeHandle,
            CancellationToken cancellationToken) where TInputPort : IInputPort<TOutputPort>;

        #endregion Methods

    }

}
