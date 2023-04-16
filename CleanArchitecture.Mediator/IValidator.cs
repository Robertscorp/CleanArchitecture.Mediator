using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// A service used to determine if it's valid for the pipeline to continue.
    /// </summary>
    /// <typeparam name="TInputPort">The type of Input Port.</typeparam>
    /// <typeparam name="TValidationResult">The type of validation result for the pipeline.</typeparam>
    public interface IValidator<TInputPort, TValidationResult>
        where TInputPort : IInputPort<IValidationOutputPort<TValidationResult>>
        where TValidationResult : IValidationResult
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Determines if it's valid for the pipeline to coninue.
        /// </summary>
        /// <param name="inputPort">The input to the pipeline.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>A validation result indicating if it's valid for the pipeline to continue.</returns>
        Task<TValidationResult> ValidateAsync(TInputPort inputPort, CancellationToken cancellationToken);

        #endregion Methods

    }

}
