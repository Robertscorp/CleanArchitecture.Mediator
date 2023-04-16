using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// An Output Port for when validation is required.
    /// </summary>
    /// <typeparam name="TValidationFailure">The type of validation failure for the pipeline.</typeparam>
    public interface IValidationOutputPort<TValidationFailure> where TValidationFailure : IValidationResult
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Presents a validation failure.
        /// </summary>
        /// <param name="validationFailure">The validation failure that occurred.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        Task PresentValidationFailureAsync(TValidationFailure validationFailure, CancellationToken cancellationToken);

        #endregion Methods

    }

}
