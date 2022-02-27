using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// An Output Port for when business rule validation is required by a Use Case.
    /// </summary>
    /// <typeparam name="TValidationFailure">The type of validation failure for the Use Case Pipeline.</typeparam>
    public interface IBusinessRuleValidationOutputPort<TValidationFailure> where TValidationFailure : IValidationResult
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Presents a business rule validation failure.
        /// </summary>
        /// <param name="validationFailure">The validation failure that occurred.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        Task PresentBusinessRuleValidationFailureAsync(TValidationFailure validationFailure, CancellationToken cancellationToken);

        #endregion Methods

    }

}
