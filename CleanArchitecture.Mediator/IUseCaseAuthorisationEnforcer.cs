using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// A service used to determine if the Use Case's Interactor is allowed to be invoked.
    /// </summary>
    /// <typeparam name="TUseCaseInputPort">The type of the Use Case's Input Port.</typeparam>
    /// <typeparam name="TAuthorisationResult">The type of authorisation result for the Use Case Pipeline.</typeparam>
    public interface IUseCaseAuthorisationEnforcer<TUseCaseInputPort, TAuthorisationResult>
        where TUseCaseInputPort : IUseCaseInputPort<IAuthorisationOutputPort<TAuthorisationResult>>
        where TAuthorisationResult : IAuthorisationResult
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Determines if the Use Case's Interactor is allowed to be invoked with the specified Input Port.
        /// </summary>
        /// <param name="inputPort">The Use Case's Input Port.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>An authorisation result indicating if the Use Case's Interactor is allowed to be invoked.</returns>
        Task<TAuthorisationResult> CheckAuthorisationAsync(TUseCaseInputPort inputPort, CancellationToken cancellationToken);

        #endregion Methods

    }

}
