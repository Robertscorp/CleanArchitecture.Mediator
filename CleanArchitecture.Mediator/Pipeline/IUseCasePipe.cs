using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Pipeline
{

    /// <summary>
    /// A single Pipe in the Use Case Pipeline.
    /// </summary>
    public interface IUseCasePipe
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Handles the relevant behaviour for this section of the Use Case Pipeline.
        /// </summary>
        /// <typeparam name="TUseCaseInputPort">The type of the Use Case's Input Port.</typeparam>
        /// <typeparam name="TUseCaseOutputPort">The type of the Use Case's Output Port.</typeparam>
        /// <param name="inputPort">The Use Case's Input Port.</param>
        /// <param name="outputPort">The Use Case's Output Port.</param>
        /// <param name="nextUseCasePipeHandle">The HandleAsync method for the next Pipe in the Use Case Pipeline.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        Task HandleAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            UseCasePipeHandleAsync nextUseCasePipeHandle,
            CancellationToken cancellationToken);

        #endregion Methods

    }

}
