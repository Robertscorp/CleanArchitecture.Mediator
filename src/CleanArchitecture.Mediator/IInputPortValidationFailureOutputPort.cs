using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Provides a mechanism for outputting input port validation failures.
    /// </summary>
    /// <typeparam name="TValidationFailure">The type of input port validation failure.</typeparam>
    public interface IInputPortValidationFailureOutputPort<TValidationFailure>
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Presents an input port validation failure.
        /// </summary>
        /// <param name="validationFailure">The <typeparamref name="TValidationFailure"/> from an <see cref="IInputPortValidator{TInputPort, TValidationFailure}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        Task PresentInputPortValidationFailureAsync(TValidationFailure validationFailure, CancellationToken cancellationToken);

        #endregion Methods

    }

}
