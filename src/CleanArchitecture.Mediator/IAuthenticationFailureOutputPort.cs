using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Represents an output port that presents authentication failures from the pipeline. 
    /// </summary>
    public interface IAuthenticationFailureOutputPort
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Presents an authentication failure from the pipeline.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        Task PresentAuthenticationFailureAsync(CancellationToken cancellationToken);

        #endregion Methods

    }

}
