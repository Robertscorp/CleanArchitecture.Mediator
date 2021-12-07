using CleanArchitecture.Services.Pipeline.Validation;

namespace CleanArchitecture.Services.Pipeline.Infrastructure
{

    public class InputPortValidatorUseCaseElement<TValidationResult> : IUseCaseElement where TValidationResult : IValidationResult
    {

        #region - - - - - - Fields - - - - - -

        private readonly IServiceProvider m_ServiceProvider;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public InputPortValidatorUseCaseElement(IServiceProvider serviceProvider)
            => this.m_ServiceProvider = serviceProvider;

        #endregion Constructors

        #region - - - - - - IUseCaseElement Implementation - - - - - -

        public async Task HandleAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            UseCaseElementHandleAsync nextUseCaseElementHandle,
            CancellationToken cancellationToken)
        {
            if (outputPort is not IValidationOutputPort<TValidationResult> _ValidationOutputPort)
            {
                await nextUseCaseElementHandle().ConfigureAwait(false);
                return;
            }

            var _Validator = (IUseCaseInputPortValidator<TUseCaseInputPort, TValidationResult>?)this.m_ServiceProvider.GetService(typeof(IUseCaseInputPortValidator<TUseCaseInputPort, TValidationResult>));
            if (_Validator == null)
            {
                await nextUseCaseElementHandle().ConfigureAwait(false);
                return;
            }

            var _ValidationResult = await _Validator.ValidateAsync(inputPort, cancellationToken).ConfigureAwait(false);
            await (_ValidationResult.IsValid
                    ? nextUseCaseElementHandle().ConfigureAwait(false)
                    : _ValidationOutputPort.PresentValidationFailureAsync(_ValidationResult, cancellationToken).ConfigureAwait(false));
        }

        #endregion IUseCaseElement Implementation

    }

}
