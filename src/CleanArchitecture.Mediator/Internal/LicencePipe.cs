using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Internal
{

    internal class LicencePipe<TInputPort, TOutputPort, TLicenceFailure> : IPipe<TInputPort, TOutputPort>
        where TInputPort : IInputPort<TOutputPort>, IInputPort<ILicenceEnforcementOutputPort<TLicenceFailure>>
        where TOutputPort : ILicenceEnforcementOutputPort<TLicenceFailure>
    {

        #region - - - - - - Methods - - - - - -

        async Task IPipe<TInputPort, TOutputPort>.InvokeAsync(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            NextPipeHandleAsync nextPipeHandle,
            CancellationToken cancellationToken)
        {
            var _LicenceVerifier = serviceFactory.GetService<ILicenceVerifier<TInputPort, TLicenceFailure>>();
            if (_LicenceVerifier != null && !await _LicenceVerifier.IsLicencedAsync(inputPort, out var _LicenceFailure, serviceFactory, cancellationToken).ConfigureAwait(false))
            {
                await outputPort.PresentLicenceFailureAsync(_LicenceFailure, cancellationToken).ConfigureAwait(false);
                return;
            }

            await nextPipeHandle().ConfigureAwait(false);
        }

        #endregion Methods

    }

}
