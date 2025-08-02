using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Provides the functionality to determine if the principal is licenced to perform this operation.
    /// </summary>
    /// <typeparam name="TInputPort">The type of input port.</typeparam>
    /// <typeparam name="TLicenceFailure">The type of licence failure.</typeparam>
    public interface ILicenceVerifier<TInputPort, TLicenceFailure> where TInputPort : IInputPort<ILicenceEnforcementOutputPort<TLicenceFailure>>
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Determines if the principal is licenced to perform this operation.
        /// </summary>
        /// <param name="inputPort">The input to the pipeline.</param>
        /// <param name="licenceFailure">The licence failure when the principal is not licenced to perform this operation.</param>
        /// <param name="serviceFactory">The factory used to get service instances.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>A result indicating if the principal is licenced to perform this operation.</returns>
        Task<bool> IsLicencedAsync(TInputPort inputPort, out TLicenceFailure licenceFailure, ServiceFactory serviceFactory, CancellationToken cancellationToken);

        #endregion Methods

    }

}
