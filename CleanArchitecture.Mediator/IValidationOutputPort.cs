using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// An Output Port for when Input Port validation is required by a Use Case.
    /// </summary>
    /// <typeparam name="TValidationFailure">The type of validation failure for the Use Case Pipeline.</typeparam>
    public interface IValidationOutputPort<TValidationFailure> where TValidationFailure : IValidationResult
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Presents an Input Port validation failure.
        /// </summary>
        /// <param name="validationFailure">The validation failure that occurred.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        Task PresentValidationFailureAsync(TValidationFailure validationFailure, CancellationToken cancellationToken);

        #endregion Methods

    }

}
