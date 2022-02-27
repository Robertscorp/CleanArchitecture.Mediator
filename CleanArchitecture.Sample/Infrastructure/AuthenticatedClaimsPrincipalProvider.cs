using CleanArchitecture.Mediator.Authentication;
using System.Security.Claims;

namespace CleanArchitecture.Sample.Infrastructure
{

    public class AuthenticatedClaimsPrincipalProvider : IAuthenticatedClaimsPrincipalProvider
    {

        #region - - - - - - Properties - - - - - -

        public ClaimsPrincipal? AuthenticatedClaimsPrincipal { get; set; }

        #endregion Properties

    }

}
