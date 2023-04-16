using System.Security.Claims;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// A service used to get the currently authenticated claims principal.
    /// </summary>
    public interface IAuthenticatedClaimsPrincipalProvider
    {

        #region - - - - - - Properties - - - - - -

        /// <summary>
        /// The currently authenticated claims principal.
        /// </summary>
        ClaimsPrincipal AuthenticatedClaimsPrincipal { get; }

        #endregion Properties

    }

}
