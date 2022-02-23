using CleanArchitecture.Services.Pipeline;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Services.Infrastructure
{

    /// <summary>
    /// Handles invocation of the Business Rule Validator service and presentation of validation failures.
    /// </summary>
    /// <typeparam name="TValidationResult">The type of Validation Result.</typeparam>
    public class BusinessRuleValidatorUseCaseElement<TValidationResult> : IUseCaseElement where TValidationResult : IValidationResult
    {

        #region - - - - - - Fields - - - - - -

        private readonly UseCaseServiceResolver m_ServiceResolver;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Initialises a new instance of the <see cref="BusinessRuleValidatorUseCaseElement{TValidationResult}"/> class.
        /// </summary>
        /// <param name="serviceResolver">The delegate used to get services.</param>
        public BusinessRuleValidatorUseCaseElement(UseCaseServiceResolver serviceResolver)
            => this.m_ServiceResolver = serviceResolver ?? throw new ArgumentNullException(nameof(serviceResolver));

        #endregion Constructors

        #region - - - - - - IUseCaseElement Implementation - - - - - -

        async Task IUseCaseElement.HandleAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            UseCaseElementHandleAsync nextUseCaseElementHandle,
            CancellationToken cancellationToken)
        {
            if (outputPort is IBusinessRuleValidationOutputPort<TValidationResult> _OutputPort)
            {
                var _ValidationResultAsync = this.GetValidationResultAsync(inputPort, cancellationToken);
                if (_ValidationResultAsync != null && !(await _ValidationResultAsync).IsValid)
                {
                    await _OutputPort.PresentBusinessRuleValidationFailureAsync(await _ValidationResultAsync, cancellationToken).ConfigureAwait(false);
                    return;
                }
            }

            await nextUseCaseElementHandle().ConfigureAwait(false);
        }

        #endregion IUseCaseElement Implementation

        #region - - - - - - Methods - - - - - -

        private Task<TValidationResult> GetValidationResultAsync<TUseCaseInputPort>(TUseCaseInputPort inputPort, CancellationToken cancellationToken)
            => inputPort is IUseCaseInputPort<IBusinessRuleValidationOutputPort<TValidationResult>>
                ? InvokeAsyncFactory<TUseCaseInputPort, TValidationResult>
                    .InvokeFactoryAsync(
                        typeof(InvokeAsyncFactory<>).MakeGenericType(typeof(TValidationResult), typeof(TUseCaseInputPort)),
                        this.m_ServiceResolver,
                        inputPort,
                        cancellationToken)
                : null;

        #endregion Methods

        #region - - - - - - Nested Classes - - - - - -

        private class InvokeAsyncFactory<TUseCaseInputPort> : InvokeAsyncFactory<TUseCaseInputPort, TValidationResult>
            where TUseCaseInputPort : IUseCaseInputPort<IBusinessRuleValidationOutputPort<TValidationResult>>
        {

            #region - - - - - - Methods - - - - - -

            public override InvokeAsync<TUseCaseInputPort, TValidationResult> GetInvokeAsync(UseCaseServiceResolver serviceResolver)
                => new InvokeAsync<TUseCaseInputPort, TValidationResult>(
                    (ip, c) => serviceResolver
                                .GetService<IUseCaseBusinessRuleValidator<TUseCaseInputPort, TValidationResult>>()?
                                .ValidateAsync(ip, c));

            #endregion Methods

        }

        #endregion Nested Classes

    }

}
