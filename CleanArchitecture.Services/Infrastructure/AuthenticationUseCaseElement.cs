﻿using CleanArchitecture.Services.Authentication;
using CleanArchitecture.Services.Pipeline;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Services.Infrastructure
{

    /// <summary>
    /// Handles authentication verification and presenting authentication failures.
    /// </summary>
    public class AuthenticationUseCaseElement : IUseCaseElement
    {

        #region - - - - - - Fields - - - - - -

        private readonly UseCaseServiceResolver m_ServiceResolver;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Initialises a new instance of the <see cref="AuthenticationUseCaseElement"/> class.
        /// </summary>
        /// <param name="serviceResolver">The delegate used to get services.</param>
        public AuthenticationUseCaseElement(UseCaseServiceResolver serviceResolver)
            => this.m_ServiceResolver = serviceResolver ?? throw new ArgumentNullException(nameof(serviceResolver));

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        private ClaimsPrincipal GetAuthenticatedClaimsPrincipal()
            => this.m_ServiceResolver.GetService<IAuthenticatedClaimsPrincipalProvider>()?.AuthenticatedClaimsPrincipal;

        Task IUseCaseElement.HandleAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            UseCaseElementHandleAsync nextUseCaseElementHandle,
            CancellationToken cancellationToken)
            => outputPort is IAuthenticationOutputPort _OutputPort && this.GetAuthenticatedClaimsPrincipal() == null
                ? _OutputPort.PresentUnauthenticatedAsync(cancellationToken)
                : nextUseCaseElementHandle();

        #endregion Methods

    }

}
