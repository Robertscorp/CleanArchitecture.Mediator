using System.Security.Claims;

namespace CleanArchitecture.Services.Authentication
{

    public interface IAuthenticatedClaimsPrincipalProvider
    {

        #region - - - - - - Properties - - - - - -

        ClaimsPrincipal? AuthenticatedClaimsPrincipal { get; }

        #endregion Properties

    }

}
