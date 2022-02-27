﻿using CleanArchitecture.Services.Pipeline;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Services.Infrastructure
{

    /// <summary>
    /// Handles invocation of the Input Port Validator service and presentation of validation failures.
    /// </summary>
    /// <typeparam name="TValidationResult">The type of Validation Result.</typeparam>
    public class InputPortValidationPipe<TValidationResult> : IUseCasePipe where TValidationResult : IValidationResult
    {

        #region - - - - - - Fields - - - - - -

        private readonly UseCaseServiceResolver m_ServiceResolver;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Initialises a new instance of the <see cref="InputPortValidationPipe{TValidationResult}"/> class.
        /// </summary>
        /// <param name="serviceResolver">The delegate used to get services.</param>
        public InputPortValidationPipe(UseCaseServiceResolver serviceResolver)
            => this.m_ServiceResolver = serviceResolver ?? throw new ArgumentNullException(nameof(serviceResolver));

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        private Task<TValidationResult> GetValidationResultAsync<TUseCaseInputPort>(TUseCaseInputPort inputPort, CancellationToken cancellationToken)
            => inputPort is IUseCaseInputPort<IValidationOutputPort<TValidationResult>>
                ? DelegateFactory
                    .GetFunction<(UseCaseServiceResolver, TUseCaseInputPort, CancellationToken), Task<TValidationResult>>(
                        typeof(ValidateFactory<>).MakeGenericType(typeof(TValidationResult), typeof(TUseCaseInputPort)))?
                    .Invoke((this.m_ServiceResolver, inputPort, cancellationToken))
                : null;

        async Task IUseCasePipe.HandleAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            UseCasePipeHandleAsync nextUseCasePipeHandle,
            CancellationToken cancellationToken)
        {
            if (outputPort is IValidationOutputPort<TValidationResult> _OutputPort)
            {
                var _ValidationResultAsync = this.GetValidationResultAsync(inputPort, cancellationToken);
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

            await nextUseCasePipeHandle().ConfigureAwait(false);
        }

        #endregion Methods

        #region - - - - - - Nested Classes - - - - - -

        private class ValidateFactory<TUseCaseInputPort>
            : IDelegateFactory<(UseCaseServiceResolver, TUseCaseInputPort, CancellationToken), Task<TValidationResult>>
            where TUseCaseInputPort : IUseCaseInputPort<IValidationOutputPort<TValidationResult>>
        {

            #region - - - - - - Methods - - - - - -

            public Func<(UseCaseServiceResolver, TUseCaseInputPort, CancellationToken), Task<TValidationResult>> GetFunction()
                => sripc
                    => sripc.Item1
                        .GetService<IUseCaseInputPortValidator<TUseCaseInputPort, TValidationResult>>()?
                        .ValidateAsync(sripc.Item2, sripc.Item3);

            #endregion Methods

        }

        #endregion Nested Classes

    }

}
