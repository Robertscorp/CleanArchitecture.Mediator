using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Internal
{

    internal class InputPortValidationPipe<TInputPort, TOutputPort, TValidationResult> : IPipe<TInputPort, TOutputPort>
        where TInputPort : IInputPort<TOutputPort>, IInputPort<IInputPortValidationOutputPort<TValidationResult>>
        where TOutputPort : IInputPortValidationOutputPort<TValidationResult>
        where TValidationResult : IInputPortValidationResult
    {

        #region - - - - - - Methods - - - - - -

        async Task IPipe<TInputPort, TOutputPort>.InvokeAsync(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            NextPipeHandleAsync nextPipeHandle,
            CancellationToken cancellationToken)
        {
            var _Validator = serviceFactory.GetService<IInputPortValidator<TInputPort, TValidationResult>>();
            var _ValidationResultAsync = _Validator?.ValidateAsync(inputPort, serviceFactory, cancellationToken);
            if (_ValidationResultAsync != null)
            {
                var _ValidationResult = await _ValidationResultAsync.ConfigureAwait(false);
                if (!_ValidationResult.IsValid)
                {
                    await outputPort.PresentInputPortValidationFailureAsync(_ValidationResult, cancellationToken).ConfigureAwait(false);
                    return;
                }
            }

            await nextPipeHandle().ConfigureAwait(false);
        }

        #endregion Methods

    }

}
