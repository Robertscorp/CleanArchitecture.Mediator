using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// A service used to determine if the pipeline is allowed to continue.
    /// </summary>
    /// <typeparam name="TInputPort">The type of input port.</typeparam>
    /// <typeparam name="TAuthorisationResult">The type of authorisation result for the pipeline.</typeparam>
    public interface IAuthorisationEnforcer<TInputPort, TAuthorisationResult>
        where TInputPort : IInputPort<IAuthorisationOutputPort<TAuthorisationResult>>
        where TAuthorisationResult : IAuthorisationResult
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Determines if the pipeline is allowed to continue with the specified input port.
        /// </summary>
        /// <param name="inputPort">The input to the pipeline.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>An authorisation result indicating if the pipeline is allowed to continue.</returns>
        Task<TAuthorisationResult> CheckAuthorisationAsync(TInputPort inputPort, CancellationToken cancellationToken);

        #endregion Methods

    }

}
