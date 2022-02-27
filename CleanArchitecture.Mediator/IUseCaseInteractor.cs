using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// The Use Case's Interactor.
    /// </summary>
    /// <typeparam name="TUseCaseInputPort">The type of the Use Case's Input Port.</typeparam>
    /// <typeparam name="TUseCaseOutputPort">The type of the Use Case's Output Port.</typeparam>
    public interface IUseCaseInteractor<TUseCaseInputPort, TUseCaseOutputPort>
        where TUseCaseInputPort : IUseCaseInputPort<TUseCaseOutputPort>
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Handles the Use Case's logic.
        /// </summary>
        /// <param name="inputPort">The Use Case's Input Port.</param>
        /// <param name="outputPort">The Use Case's Output Port.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        Task HandleAsync(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            CancellationToken cancellationToken);

        #endregion Methods

    }

}
