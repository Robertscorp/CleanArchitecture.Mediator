namespace CleanArchitecture.Mediator.Sample.Legacy.InputPortValidation
{

    /// <summary>
    /// Handles invocation of the validator service and presentation of validation failures.
    /// </summary>
    /// <typeparam name="TValidationResult">The type of validation result.</typeparam>
    public class InputPortValidationPipe<TValidationResult> : IPipe where TValidationResult : IInputPortValidationResult
    {

        #region - - - - - - Methods - - - - - -

        private static Task<TValidationResult>? GetValidationResultAsync<TInputPort>(TInputPort inputPort, ServiceFactory serviceFactory, CancellationToken cancellationToken)
            => inputPort is IInputPort<IInputPortValidationOutputPort<TValidationResult>>
                ? DelegateCache
                    .GetFunction<(ServiceFactory, TInputPort, CancellationToken), Task<TValidationResult>?>(
                        typeof(ValidateFactory<>).MakeGenericType(typeof(TValidationResult), typeof(TInputPort)))?
                    .Invoke((serviceFactory, inputPort, cancellationToken))
                : null;

        async Task IPipe.InvokeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            IPipeHandle nextPipeHandle,
            CancellationToken cancellationToken)
        {
            if (outputPort is IInputPortValidationOutputPort<TValidationResult> _OutputPort)
            {
                var _ValidationResultAsync = GetValidationResultAsync(inputPort, serviceFactory, cancellationToken);
                if (_ValidationResultAsync != null)
                {
                    var _ValidationResult = await _ValidationResultAsync.ConfigureAwait(false);
                    if (!_ValidationResult.IsValid)
                    {
                        await _OutputPort.PresentValidationFailureAsync(_ValidationResult, cancellationToken).ConfigureAwait(false);
                        return;
                    }
                }
            }

            await nextPipeHandle.InvokePipeAsync(inputPort, outputPort, serviceFactory, cancellationToken).ConfigureAwait(false);
        }

        #endregion Methods

        #region - - - - - - Nested Classes - - - - - -

        private class ValidateFactory<TInputPort>
            : IDelegateProvider<(ServiceFactory, TInputPort, CancellationToken), Task<TValidationResult>?>
            where TInputPort : IInputPort<IInputPortValidationOutputPort<TValidationResult>>
        {

            #region - - - - - - Methods - - - - - -

            public Func<(ServiceFactory, TInputPort, CancellationToken), Task<TValidationResult>?> GetFunction()
                => tuple
                    => tuple.Item1
                        .GetService<IInputPortValidator<TInputPort, TValidationResult>>()?
                        .ValidateAsync(tuple.Item2, tuple.Item3);

            #endregion Methods

        }

        #endregion Nested Classes

    }

}
