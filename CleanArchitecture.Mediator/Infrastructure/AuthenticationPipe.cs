using CleanArchitecture.Mediator.Authentication;
using CleanArchitecture.Mediator.Pipeline;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Infrastructure
{

    /// <summary>
    /// Handles authentication verification and presenting authentication failures.
    /// </summary>
    public class AuthenticationPipe : IUseCasePipe
    {

        #region - - - - - - Fields - - - - - -

        private readonly UseCaseServiceResolver m_ServiceResolver;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Initialises a new instance of the <see cref="AuthenticationPipe"/> class.
        /// </summary>
        /// <param name="serviceResolver">The delegate used to get services.</param>
        public AuthenticationPipe(UseCaseServiceResolver serviceResolver)
            => this.m_ServiceResolver = serviceResolver ?? throw new ArgumentNullException(nameof(serviceResolver));

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        private ClaimsPrincipal GetAuthenticatedClaimsPrincipal()
            => this.m_ServiceResolver.GetService<IAuthenticatedClaimsPrincipalProvider>()?.AuthenticatedClaimsPrincipal;

        Task IUseCasePipe.HandleAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            UseCasePipeHandleAsync nextUseCasePipeHandle,
            CancellationToken cancellationToken)
            => outputPort is IAuthenticationOutputPort _OutputPort && this.GetAuthenticatedClaimsPrincipal() == null
                ? _OutputPort.PresentUnauthenticatedAsync(cancellationToken)
                : nextUseCasePipeHandle();

        #endregion Methods

    }

}
