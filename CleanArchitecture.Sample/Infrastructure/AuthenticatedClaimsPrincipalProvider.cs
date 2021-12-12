using CleanArchitecture.Services.Pipeline.Authentication;
using System.Security.Claims;

namespace CleanArchitecture.Sample.Infrastructure
{

    public class AuthenticatedClaimsPrincipalProvider : IAuthenticatedClaimsPrincipalProvider
    {

        #region - - - - - - IAuthenticatedClaimsPrincipalProvider Implementation - - - - - -

        public ClaimsPrincipal? AuthenticatedClaimsPrincipal { get; set; }

        #endregion IAuthenticatedClaimsPrincipalProvider Implementation

    }

}
