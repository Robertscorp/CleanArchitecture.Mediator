using CleanArchitecture.Services.Pipeline.Authorisation;

namespace CleanArchitecture.Services.Pipeline.Infrastructure
{

    public class AuthorisationUseCaseElement<TAuthorisationResult> : IUseCaseElement where TAuthorisationResult : IAuthorisationResult
    {

        #region - - - - - - Fields - - - - - -

        private readonly IServiceProvider m_ServiceProvider;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public AuthorisationUseCaseElement(IServiceProvider serviceProvider)
            => this.m_ServiceProvider = serviceProvider;

        #endregion Constructors

        #region - - - - - - IUseCaseElement Implementation - - - - - -

        public async Task HandleAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            UseCaseElementHandleAsync nextUseCaseElementHandle,
            CancellationToken cancellationToken)
        {
            if (outputPort is not IAuthorisationOutputPort<TAuthorisationResult> _OutputPort)
            {
                await nextUseCaseElementHandle().ConfigureAwait(false);
                return;
            }

            var _Enforcer = (IUseCaseAuthorisationEnforcer<TUseCaseInputPort, TAuthorisationResult>?)this.m_ServiceProvider.GetService(typeof(IUseCaseAuthorisationEnforcer<TUseCaseInputPort, TAuthorisationResult>));
            if (_Enforcer == null)
            {
                await nextUseCaseElementHandle().ConfigureAwait(false);
                return;
            }

            var _AuthorisationResult = await _Enforcer.CheckAuthorisationAsync(inputPort, cancellationToken).ConfigureAwait(false);
            await (_AuthorisationResult.IsAuthorised
                    ? nextUseCaseElementHandle().ConfigureAwait(false)
                    : _OutputPort.PresentUnauthorisedAsync(_AuthorisationResult, cancellationToken).ConfigureAwait(false));
        }

        #endregion IUseCaseElement Implementation

    }

}
