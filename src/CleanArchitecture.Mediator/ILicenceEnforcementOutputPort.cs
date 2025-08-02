using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Provides a mechanism for outputting licence failures.
    /// </summary>
    /// <typeparam name="TLicenceFailure">The type of licence failure.</typeparam>
    public interface ILicenceEnforcementOutputPort<TLicenceFailure>
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Presents a licence failure.
        /// </summary>
        /// <param name="licenceFailure">The licence failure from an <see cref="ILicenceVerifier{TInputPort, TLicenceFailure}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        Task PresentLicenceFailureAsync(TLicenceFailure licenceFailure, CancellationToken cancellationToken);

        #endregion Methods

    }

}
