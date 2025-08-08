using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Provides a mechanism for outputting authorisation policy failures.
    /// </summary>
    /// <typeparam name="TPolicyFailure">The type of authorisation policy failure.</typeparam>
    public interface IAuthorisationPolicyFailureOutputPort<TPolicyFailure>
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Presents an authorisation policy failure.
        /// </summary>
        /// <param name="policyFailure">The <typeparamref name="TPolicyFailure"/> from an <see cref="IAuthorisationPolicyValidator{TInputPort, TPolicyFailure}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        Task PresentAuthorisationPolicyFailureAsync(TPolicyFailure policyFailure, CancellationToken cancellationToken);

        #endregion Methods

    }

}
