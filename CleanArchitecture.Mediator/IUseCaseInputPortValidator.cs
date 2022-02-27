using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// A service used to determine if the Use Case's Input Port is valid.
    /// </summary>
    /// <typeparam name="TUseCaseInputPort">The type of the Use Case's Input Port.</typeparam>
    /// <typeparam name="TValidationResult">The type of validation result for the Use Case Pipeline.</typeparam>
    public interface IUseCaseInputPortValidator<TUseCaseInputPort, TValidationResult>
        where TUseCaseInputPort : IUseCaseInputPort<IValidationOutputPort<TValidationResult>>
        where TValidationResult : IValidationResult
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Determines if the specified Input Port is valid.
        /// </summary>
        /// <param name="inputPort">The Use Case's Input Port.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A validation result indicating if the Input Port is valid.</returns>
        Task<TValidationResult> ValidateAsync(TUseCaseInputPort inputPort, CancellationToken cancellationToken);

        #endregion Methods

    }

}
