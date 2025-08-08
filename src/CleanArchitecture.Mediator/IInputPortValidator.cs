using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Provides the functionality to determine if the input port is valid.
    /// </summary>
    /// <typeparam name="TInputPort">The type of input port.</typeparam>
    /// <typeparam name="TValidationFailure">The type of input port validation failure.</typeparam>
    public interface IInputPortValidator<TInputPort, TValidationFailure>
        where TInputPort : IInputPort<IInputPortValidationFailureOutputPort<TValidationFailure>>
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Determines if the specified <paramref name="inputPort"/> is valid.
        /// </summary>
        /// <param name="inputPort">The input to the pipeline.</param>
        /// <param name="validationFailure">The <typeparamref name="TValidationFailure"/> provided when the <paramref name="inputPort"/> is invalid.</param>
        /// <param name="serviceFactory">The <see cref="ServiceFactory"/> used to get service instances.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>A result indicating if the specified <paramref name="inputPort"/> is valid.</returns>
        Task<bool> ValidateAsync(TInputPort inputPort, out TValidationFailure validationFailure, ServiceFactory serviceFactory, CancellationToken cancellationToken);

        #endregion Methods

    }

}
