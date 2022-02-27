using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// A service used to determine if the Use Case's Interactor will violate business rules.
    /// </summary>
    /// <typeparam name="TUseCaseInputPort">The type of the Use Case's Input Port.</typeparam>
    /// <typeparam name="TValidationResult">The type of validation result for the Use Case Pipeline.</typeparam>
    public interface IUseCaseBusinessRuleValidator<TUseCaseInputPort, TValidationResult>
        where TUseCaseInputPort : IUseCaseInputPort<IBusinessRuleValidationOutputPort<TValidationResult>>
        where TValidationResult : IValidationResult
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Determines if the Use Case's Interactor will violate business rules if invoked with the specified Input Port.
        /// </summary>
        /// <param name="inputPort">The Use Case's Input Port.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A validation result indicating if the Use Case's Interactor will violate business rules.</returns>
        Task<TValidationResult> ValidateAsync(TUseCaseInputPort inputPort, CancellationToken cancellationToken);

        #endregion Methods

    }

}
