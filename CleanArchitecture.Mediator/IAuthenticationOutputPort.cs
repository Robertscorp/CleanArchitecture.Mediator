using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// An Output Port for when authentication is required by a Use Case.
    /// </summary>
    public interface IAuthenticationOutputPort
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Presents an authentication failure.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        Task PresentUnauthenticatedAsync(CancellationToken cancellationToken);

        #endregion Methods

    }

}
