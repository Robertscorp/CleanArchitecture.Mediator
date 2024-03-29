﻿using CleanArchitecture.Mediator.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Pipes
{

    /// <summary>
    /// Handles invocation of the validator service and presentation of validation failures.
    /// </summary>
    /// <typeparam name="TValidationResult">The type of validation result.</typeparam>
    public class ValidationPipe<TValidationResult> : IPipe where TValidationResult : IValidationResult
    {

        #region - - - - - - Methods - - - - - -

        private Task<TValidationResult> GetValidationResultAsync<TInputPort>(TInputPort inputPort, ServiceFactory serviceFactory, CancellationToken cancellationToken)
            => inputPort is IInputPort<IValidationOutputPort<TValidationResult>>
                ? DelegateFactory
                    .GetFunction<(ServiceFactory, TInputPort, CancellationToken), Task<TValidationResult>>(
                        typeof(ValidateFactory<>).MakeGenericType(typeof(TValidationResult), typeof(TInputPort)))?
                    .Invoke((serviceFactory, inputPort, cancellationToken))
                : null;

        async Task IPipe.InvokeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            PipeHandle nextPipeHandle,
            CancellationToken cancellationToken)
        {
            if (outputPort is IValidationOutputPort<TValidationResult> _OutputPort)
            {
                var _ValidationResultAsync = this.GetValidationResultAsync(inputPort, serviceFactory, cancellationToken);
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
            : IDelegateFactory<(ServiceFactory, TInputPort, CancellationToken), Task<TValidationResult>>
            where TInputPort : IInputPort<IValidationOutputPort<TValidationResult>>
        {

            #region - - - - - - Methods - - - - - -

            public Func<(ServiceFactory, TInputPort, CancellationToken), Task<TValidationResult>> GetFunction()
                => tuple
                    => tuple.Item1
                        .GetService<IValidator<TInputPort, TValidationResult>>()?
                        .ValidateAsync(tuple.Item2, tuple.Item3);

            #endregion Methods

        }

        #endregion Nested Classes

    }

}
