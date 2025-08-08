using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Internal
{

    internal class InputPortValidationPipe<TInputPort, TOutputPort, TValidationFailure> : IPipe<TInputPort, TOutputPort>
        where TInputPort : IInputPort<TOutputPort>, IInputPort<IInputPortValidationFailureOutputPort<TValidationFailure>>
        where TOutputPort : IInputPortValidationFailureOutputPort<TValidationFailure>
    {

        #region - - - - - - Methods - - - - - -

        async Task IPipe<TInputPort, TOutputPort>.InvokeAsync(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            NextPipeHandleAsync nextPipeHandle,
            CancellationToken cancellationToken)
        {
            var _Validator = serviceFactory.GetService<IInputPortValidator<TInputPort, TValidationFailure>>();
            if (_Validator != null && !await _Validator.ValidateAsync(inputPort, out var _ValidationFailure, serviceFactory, cancellationToken))
            {
                await outputPort.PresentInputPortValidationFailureAsync(_ValidationFailure, cancellationToken).ConfigureAwait(false);
                return;
            }

            await nextPipeHandle().ConfigureAwait(false);
        }

        #endregion Methods

    }

}
