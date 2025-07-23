using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Provides the functionality to determine if the input port is valid.
    /// </summary>
    /// <typeparam name="TInputPort">The type of input port.</typeparam>
    /// <typeparam name="TValidationResult">The type of input port validation result.</typeparam>
    public interface IInputPortValidator<TInputPort, TValidationResult>
        where TInputPort : IInputPort<IInputPortValidationOutputPort<TValidationResult>>
        where TValidationResult : IInputPortValidationResult
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Determines if the specified <paramref name="inputPort"/> is valid.
        /// </summary>
        /// <param name="inputPort">The input to the pipeline.</param>
        /// <param name="serviceFactory">The <see cref="ServiceFactory"/> used to get service instances.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>A validation result indicating if the specified <paramref name="inputPort"/> is valid.</returns>
        Task<TValidationResult> ValidateAsync(TInputPort inputPort, ServiceFactory serviceFactory, CancellationToken cancellationToken);

        #endregion Methods

    }

}
