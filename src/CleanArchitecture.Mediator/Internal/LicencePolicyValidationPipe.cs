using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Internal
{

    internal class LicencePolicyValidationPipe<TInputPort, TOutputPort, TPolicyFailure> : IPipe<TInputPort, TOutputPort>
        where TInputPort : IInputPort<TOutputPort>, IInputPort<ILicencePolicyFailureOutputPort<TPolicyFailure>>
        where TOutputPort : ILicencePolicyFailureOutputPort<TPolicyFailure>
    {

        #region - - - - - - Methods - - - - - -

        async Task IPipe<TInputPort, TOutputPort>.InvokeAsync(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            NextPipeHandleAsync nextPipeHandle,
            CancellationToken cancellationToken)
        {
            var _Validator = serviceFactory.GetService<ILicencePolicyValidator<TInputPort, TPolicyFailure>>();
            if (!await _Validator.ValidateAsync(inputPort, out var _PolicyFailure, serviceFactory, cancellationToken).ConfigureAwait(false))
            {
                var _ContinuationBehaviour = await outputPort.PresentLicencePolicyFailureAsync(_PolicyFailure, cancellationToken).ConfigureAwait(false);
                await _ContinuationBehaviour.HandleAsync(nextPipeHandle, cancellationToken).ConfigureAwait(false);
            }
            else
                await nextPipeHandle().ConfigureAwait(false);
        }

        #endregion Methods

    }

}
