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

        public async Task<bool> TryOutputResultAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            CancellationToken cancellationToken)
        {
            if (outputPort is not IAuthorisationOutputPort<TAuthorisationResult> _OutputPort)
                return false;

            var _Enforcer = (IUseCaseAuthorisationEnforcer<TUseCaseInputPort, TAuthorisationResult>?)this.m_ServiceProvider.GetService(typeof(IUseCaseAuthorisationEnforcer<TUseCaseInputPort, TAuthorisationResult>));
            if (_Enforcer == null)
                return false;

            var _AuthorisationResult = await _Enforcer.CheckAuthorisationAsync(inputPort, cancellationToken);
            if (_AuthorisationResult.IsAuthorised)
                return false;

            await _OutputPort.PresentUnauthorisedAsync(_AuthorisationResult, cancellationToken).ConfigureAwait(false);

            return true;
        }

        #endregion IUseCaseElement Implementation

    }

}
