using CleanArchitecture.Services.Pipeline.Validation;

namespace CleanArchitecture.Services.Pipeline.Infrastructure
{

    public class BusinessRuleValidatorUseCaseElement<TValidationResult> : IUseCaseElement where TValidationResult : IValidationResult
    {

        #region - - - - - - Fields - - - - - -

        private readonly IServiceProvider m_ServiceProvider;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public BusinessRuleValidatorUseCaseElement(IServiceProvider serviceProvider)
            => this.m_ServiceProvider = serviceProvider;

        #endregion Constructors

        #region - - - - - - IUseCaseElement Implementation - - - - - -

        public async Task HandleAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            UseCaseElementHandleAsync nextUseCaseElementHandle,
            CancellationToken cancellationToken)
        {
            if (outputPort is not IBusinessRuleValidationOutputPort<TValidationResult> _ValidationOutputPort)
            {
                await nextUseCaseElementHandle().ConfigureAwait(false);
                return;
            }

            var _Validator = (IUseCaseBusinessRuleValidator<TUseCaseInputPort, TValidationResult>?)this.m_ServiceProvider.GetService(typeof(IUseCaseBusinessRuleValidator<TUseCaseInputPort, TValidationResult>));
            if (_Validator == null)
            {
                await nextUseCaseElementHandle().ConfigureAwait(false);
                return;
            }

            var _ValidationResult = await _Validator.ValidateAsync(inputPort, cancellationToken).ConfigureAwait(false);
            await (_ValidationResult.IsValid
                    ? nextUseCaseElementHandle().ConfigureAwait(false)
                    : _ValidationOutputPort.PresentBusinessRuleValidationFailureAsync(_ValidationResult, cancellationToken).ConfigureAwait(false));
        }

        #endregion IUseCaseElement Implementation

    }

}
