using System.Security.Claims;

namespace CleanArchitecture.Services.Pipeline.Authentication
{

    public interface IAuthenticatedClaimsPrincipalProvider
    {

        #region - - - - - - Properties - - - - - -

        ClaimsPrincipal AuthenticatedClaimsPrincipal { get; }

        #endregion Properties

    }

}
