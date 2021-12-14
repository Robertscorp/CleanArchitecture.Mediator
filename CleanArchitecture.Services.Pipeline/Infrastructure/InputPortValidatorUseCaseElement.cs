using CleanArchitecture.Services.Pipeline.Validation;

namespace CleanArchitecture.Services.Pipeline.Infrastructure
{

    public class InputPortValidatorUseCaseElement<TValidationResult> : IUseCaseElement where TValidationResult : IValidationResult
    {

        #region - - - - - - Fields - - - - - -

        private readonly UseCaseServiceResolver m_ServiceResolver;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public InputPortValidatorUseCaseElement(UseCaseServiceResolver serviceResolver)
            => this.m_ServiceResolver = serviceResolver;

        #endregion Constructors

        #region - - - - - - IUseCaseElement Implementation - - - - - -

        public async Task HandleAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            UseCaseElementHandleAsync nextUseCaseElementHandle,
            CancellationToken cancellationToken)
        {
            if (outputPort is IValidationOutputPort<TValidationResult> _OutputPort)
            {
                var _ValidationResultAsync = this.GetValidationResultAsync(inputPort, cancellationToken);
                if (_ValidationResultAsync != null && !(await _ValidationResultAsync).IsValid)
                {
                    await _OutputPort.PresentValidationFailureAsync(await _ValidationResultAsync, cancellationToken).ConfigureAwait(false);
                    return;
                }
            }

            await nextUseCaseElementHandle().ConfigureAwait(false);
        }

        #endregion IUseCaseElement Implementation

        #region - - - - - - Methods - - - - - -

        private Task<TValidationResult>? GetValidationResultAsync<TUseCaseInputPort>(TUseCaseInputPort inputPort, CancellationToken cancellationToken)
            => inputPort is IUseCaseInputPort<IValidationOutputPort<TValidationResult>>
                ? this.GetValidationInvoker(inputPort)?.GetValidationResultAsync(cancellationToken)
                : null;

        private ValidationInvoker? GetValidationInvoker<TUseCaseInputPort>(TUseCaseInputPort inputPort)
            => (ValidationInvoker?)Activator.CreateInstance(
                typeof(ValidationInvoker<>).MakeGenericType(typeof(TValidationResult), typeof(TUseCaseInputPort)),
                inputPort,
                this.m_ServiceResolver);

        #endregion Methods

        #region - - - - - - Nested Classes - - - - - -

        private abstract class ValidationInvoker
        {

            #region - - - - - - Methods - - - - - -

            public abstract Task<TValidationResult>? GetValidationResultAsync(CancellationToken cancellationToken);

            #endregion Methods

        }

        private class ValidationInvoker<TUseCaseInputPort> : ValidationInvoker
            where TUseCaseInputPort : IUseCaseInputPort<IValidationOutputPort<TValidationResult>>
        {

            #region - - - - - - Fields - - - - - -

            private readonly TUseCaseInputPort m_InputPort;
            private readonly UseCaseServiceResolver m_ServiceResolver;

            #endregion Fields

            #region - - - - - - Constructors - - - - - -

            public ValidationInvoker(TUseCaseInputPort inputPort, UseCaseServiceResolver serviceResolver)
            {
                this.m_InputPort = inputPort;
                this.m_ServiceResolver = serviceResolver;
            }

            #endregion Constructors

            #region - - - - - - Methods - - - - - -

            public override Task<TValidationResult>? GetValidationResultAsync(CancellationToken cancellationToken)
                => this.m_ServiceResolver
                    .GetService<IUseCaseInputPortValidator<TUseCaseInputPort, TValidationResult>>()?
                    .ValidateAsync(this.m_InputPort, cancellationToken);

            #endregion Methods

        }

        #endregion Nested Classes

    }

}
