using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Internal
{

    internal class AuthorisationPolicyValidationPipe<TInputPort, TOutputPort, TPolicyFailure> : IPipe<TInputPort, TOutputPort>
        where TInputPort : IInputPort<TOutputPort>, IInputPort<IAuthorisationPolicyFailureOutputPort<TPolicyFailure>>
        where TOutputPort : IAuthorisationPolicyFailureOutputPort<TPolicyFailure>
    {

        #region - - - - - - Methods - - - - - -

        async Task IPipe<TInputPort, TOutputPort>.InvokeAsync(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            NextPipeHandleAsync nextPipeHandle,
            CancellationToken cancellationToken)
        {
            var _Validator = serviceFactory.GetService<IAuthorisationPolicyValidator<TInputPort, TPolicyFailure>>();
            if (_Validator != null && !await _Validator.ValidateAsync(inputPort, out var _PolicyFailure, serviceFactory, cancellationToken).ConfigureAwait(false))
            {
                await outputPort.PresentAuthorisationPolicyFailureAsync(_PolicyFailure, cancellationToken).ConfigureAwait(false);
                return;
            }

            await nextPipeHandle().ConfigureAwait(false);
        }

        #endregion Methods

    }

}
