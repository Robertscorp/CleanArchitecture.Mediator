using System.Security.Claims;

namespace CleanArchitecture.Services.Authentication
{

    /// <summary>
    /// A service used to get the currently authenticated Claims Principal.
    /// </summary>
    public interface IAuthenticatedClaimsPrincipalProvider
    {

        #region - - - - - - Properties - - - - - -

        /// <summary>
        /// The currently authenticated Claims Principal.
        /// </summary>
        ClaimsPrincipal AuthenticatedClaimsPrincipal { get; }

        #endregion Properties

    }

}
