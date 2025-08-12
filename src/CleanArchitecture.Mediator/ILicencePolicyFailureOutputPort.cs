using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Provides a mechanism for outputting licence policy failures.
    /// </summary>
    /// <typeparam name="TPolicyFailure">The type of licence policy failure.</typeparam>
    public interface ILicencePolicyFailureOutputPort<TPolicyFailure>
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Presents a licence policy failure.
        /// </summary>
        /// <param name="policyFailure">The <typeparamref name="TPolicyFailure"/> from an <see cref="ILicencePolicyValidator{TInputPort, TPolicyFailure}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>A continuation strategy that determines how the pipeline should proceed.</returns>
        Task<ContinuationBehaviour> PresentLicencePolicyFailureAsync(TPolicyFailure policyFailure, CancellationToken cancellationToken);

        #endregion Methods

    }

}
