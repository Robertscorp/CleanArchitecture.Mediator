using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Provides a mechanism for outputting authorisation failures.
    /// </summary>
    /// <typeparam name="TAuthorisationFailure">The type of authorisation failure.</typeparam>
    public interface IAuthorisationOutputPort<TAuthorisationFailure>
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Presents an authorisation failure.
        /// </summary>
        /// <param name="authorisationFailure">The authorisation failure from an <see cref="IAuthorisationEnforcer{TInputPort, TAuthorisationFailure}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        Task PresentAuthorisationFailureAsync(TAuthorisationFailure authorisationFailure, CancellationToken cancellationToken);

        #endregion Methods

    }

}
