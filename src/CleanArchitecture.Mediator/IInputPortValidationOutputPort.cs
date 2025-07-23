using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Provides a mechanism for outputting input port validation failures.
    /// </summary>
    /// <typeparam name="TValidationResult">The type of input port validation result.</typeparam>
    public interface IInputPortValidationOutputPort<TValidationResult> where TValidationResult : IInputPortValidationResult
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Presents an input port validation failure.
        /// </summary>
        /// <param name="validationResult">The result from an <see cref="IInputPortValidator{TInputPort, TValidationResult}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        Task PresentInputPortValidationFailureAsync(TValidationResult validationResult, CancellationToken cancellationToken);

        #endregion Methods

    }

}
