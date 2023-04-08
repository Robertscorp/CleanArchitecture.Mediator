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
        /// <typeparam name="TUseCaseInputPort">The type of the Use Case's Input Port.</typeparam>
        /// <typeparam name="TUseCaseOutputPort">The type of the Use Case's Output Port.</typeparam>
        /// <param name="inputPort">The Use Case's Input Port.</param>
        /// <param name="outputPort">The Use Case's Output Port.</param>
        /// <param name="serviceFactory">The factory used to get the service object of the specified type.</param>
        /// <param name="nextPipeHandle">The handle to the next Pipe in the Pipeline.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        Task InvokeAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            ServiceFactory serviceFactory,
            PipeHandle nextPipeHandle,
            CancellationToken cancellationToken);

        #endregion Methods

    }

}
