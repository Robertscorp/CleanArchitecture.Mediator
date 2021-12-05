using CleanArchitecture.Services.Extended.Validation;
using CleanArchitecture.Services.Pipeline;

namespace CleanArchitecture.Services.Extended.Infrastructure
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

        public async Task<bool> TryOutputResultAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            CancellationToken cancellationToken)
        {
            if (outputPort is not IValidationOutputPort<TValidationResult> _ValidationOutputPort)
                return false;

            var _Validator = (IUseCaseInputPortValidator<TUseCaseInputPort, TValidationResult>?)this.m_ServiceProvider.GetService(typeof(IUseCaseInputPortValidator<TUseCaseInputPort, TValidationResult>));
            if (_Validator == null)
                return false;

            var _ValidationResult = await _Validator.ValidateAsync(inputPort, cancellationToken).ConfigureAwait(false);
            if (_ValidationResult.IsValid)
                return false;

            await _ValidationOutputPort.PresentValidationFailureAsync(_ValidationResult, cancellationToken).ConfigureAwait(false);

            return true;
        }

        #endregion IUseCaseElement Implementation

    }

}
