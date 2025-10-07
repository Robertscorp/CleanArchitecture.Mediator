using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Provides the functionality to determine if the principal is authorised to perform this operation.
    /// </summary>
    /// <typeparam name="TInputPort">The type of input port.</typeparam>
    /// <typeparam name="TPolicyFailure">The type of authorisation policy failure.</typeparam>
    public interface IAuthorisationPolicyValidator<TInputPort, TPolicyFailure> where TInputPort : IInputPort<IAuthorisationPolicyFailureOutputPort<TPolicyFailure>>
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Determines if the principal is authorised to perform this operation.
        /// </summary>
        /// <param name="inputPort">The input to the pipeline.</param>
        /// <param name="policyFailure">The <typeparamref name="TPolicyFailure"/> provided when the principal is not authorised to perform this operation.</param>
        /// <param name="serviceFactory">The <see cref="ServiceFactory"/> used to get service instances.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>A result indicating if the principal is authorised to perform this operation.</returns>
        Task<bool> ValidateAsync(TInputPort inputPort, out TPolicyFailure policyFailure, ServiceFactory serviceFactory, CancellationToken cancellationToken);

        #endregion Methods

    }

}
