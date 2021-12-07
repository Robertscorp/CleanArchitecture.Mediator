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

        public Task HandleAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            UseCaseElementHandleAsync nextUseCaseElementHandle,
            CancellationToken cancellationToken)
        {
            if (outputPort is not IAuthenticationOutputPort _OutputPort)
                return nextUseCaseElementHandle();

            var _AuthenticatedClaimsPrincipalProvider = (IAuthenticatedClaimsPrincipalProvider?)this.m_ServiceProvider.GetService(typeof(IAuthenticatedClaimsPrincipalProvider));
            return _AuthenticatedClaimsPrincipalProvider?.AuthenticatedClaimsPrincipal == null
                ? _OutputPort.PresentUnauthenticatedAsync(cancellationToken)
                : nextUseCaseElementHandle();
        }

        #endregion IUseCaseElement Implementation

    }

}
