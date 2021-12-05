using CleanArchitecture.Services.Pipeline.Authentication;

namespace CleanArchitecture.Services.Pipeline.Infrastructure
{

    public class AuthenticationUseCaseElement : IUseCaseElement
    {

        #region - - - - - - Fields - - - - - -

        private readonly IServiceProvider m_ServiceProvider;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public AuthenticationUseCaseElement(IServiceProvider serviceProvider)
            => this.m_ServiceProvider = serviceProvider;

        #endregion Constructors

        #region - - - - - - IUseCaseElement Implementation - - - - - -

        public async Task<bool> TryOutputResultAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            CancellationToken cancellationToken)
        {
            if (outputPort is not IAuthenticationOutputPort _OutputPort)
                return false;

            var _AuthenticatedClaimsPrincipalProvider = (IAuthenticatedClaimsPrincipalProvider?)this.m_ServiceProvider.GetService(typeof(IAuthenticatedClaimsPrincipalProvider));
            var _AuthenticatedClaimsPrincipal = _AuthenticatedClaimsPrincipalProvider?.AuthenticatedClaimsPrincipal;
            if (_AuthenticatedClaimsPrincipal != null)
                return false;

            await _OutputPort.PresentUnauthenticatedAsync(cancellationToken).ConfigureAwait(false);

            return true;
        }

        #endregion IUseCaseElement Implementation

    }

}
