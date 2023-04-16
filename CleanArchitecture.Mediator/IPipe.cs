using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// A single pipe in the pipeline.
    /// </summary>
    public interface IPipe
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Handles the relevant behaviour for this section of the pipeline.
        /// </summary>
        /// <typeparam name="TInputPort">The type of input port.</typeparam>
        /// <typeparam name="TOutputPort">The type of output port.</typeparam>
        /// <param name="inputPort">The input to the pipeline.</param>
        /// <param name="outputPort">The output mechanism for the pipeline.</param>
        /// <param name="serviceFactory">The factory used to get the service object of the specified type.</param>
        /// <param name="nextPipeHandle">The handle to the next pipe in the pipeline.</param>
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
