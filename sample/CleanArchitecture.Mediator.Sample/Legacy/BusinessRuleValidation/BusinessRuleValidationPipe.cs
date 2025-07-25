﻿namespace CleanArchitecture.Mediator.Sample.Legacy.BusinessRuleValidation;

/// <summary>
/// Handles invocation of the validator service and presentation of validation failures.
/// </summary>
/// <typeparam name="TValidationResult">The type of validation result.</typeparam>
public class BusinessRuleValidationPipe<TValidationResult> : IPipe where TValidationResult : IBusinessRuleValidationResult
{

    #region - - - - - - Methods - - - - - -

    private static Task<TValidationResult>? GetValidationResultAsync<TInputPort>(TInputPort inputPort, ServiceFactory serviceFactory, CancellationToken cancellationToken)
        => inputPort is IInputPort<IBusinessRuleValidationOutputPort<TValidationResult>>
            ? DelegateCache
                .GetFunction<(ServiceFactory, TInputPort, CancellationToken), Task<TValidationResult>?>(
                    typeof(ValidateFactory<>).MakeGenericType(typeof(TValidationResult), typeof(TInputPort)))?
                .Invoke((serviceFactory, inputPort, cancellationToken))
            : null;

    async Task IPipe.InvokeAsync<TInputPort, TOutputPort>(
        TInputPort inputPort,
        TOutputPort outputPort,
        ServiceFactory serviceFactory,
        NextPipeHandleAsync nextPipeHandle,
        CancellationToken cancellationToken)
    {
        if (outputPort is IBusinessRuleValidationOutputPort<TValidationResult> _OutputPort)
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

        await nextPipeHandle().ConfigureAwait(false);
    }

    #endregion Methods

    #region - - - - - - Nested Classes - - - - - -

    private class ValidateFactory<TInputPort>
        : IDelegateProvider<(ServiceFactory, TInputPort, CancellationToken), Task<TValidationResult>?>
        where TInputPort : IInputPort<IBusinessRuleValidationOutputPort<TValidationResult>>
    {

        #region - - - - - - Methods - - - - - -

        public Func<(ServiceFactory, TInputPort, CancellationToken), Task<TValidationResult>?> GetFunction()
            => tuple
                => tuple.Item1
                    .GetService<IBusinessRuleValidator<TInputPort, TValidationResult>>()?
                    .ValidateAsync(tuple.Item2, tuple.Item3);

        #endregion Methods

    }

    #endregion Nested Classes

}
