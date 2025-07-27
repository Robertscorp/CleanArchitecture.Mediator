using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Provides the functionality to determine if the principal is authorised to perform this operation.
    /// </summary>
    /// <typeparam name="TInputPort">The type of input port.</typeparam>
    /// <typeparam name="TAuthorisationFailure">The type of authorisation failure.</typeparam>
    public interface IAuthorisationEnforcer<TInputPort, TAuthorisationFailure> where TInputPort : IInputPort<IAuthorisationOutputPort<TAuthorisationFailure>>
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Determines if the principal is authorised to perform this operation.
        /// </summary>
        /// <param name="inputPort">The input to the pipeline.</param>
        /// <param name="authorisationFailure">The authorisation failure when the principal is not authorised to perform this operation.</param>
        /// <param name="serviceFactory">The factory used to get service instances.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>A result indicating if the principal is authorised to perform this operation.</returns>
        Task<bool> IsAuthorisedAsync(TInputPort inputPort, out TAuthorisationFailure authorisationFailure, ServiceFactory serviceFactory, CancellationToken cancellationToken);

        #endregion Methods

    }

}
