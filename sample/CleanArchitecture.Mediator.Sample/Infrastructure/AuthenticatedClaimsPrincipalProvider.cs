using System.Security.Claims;

namespace CleanArchitecture.Mediator.Sample.Infrastructure;

public class AuthenticatedClaimsPrincipalProvider : IAuthenticatedClaimsPrincipalProvider
{

    #region - - - - - - Properties - - - - - -

    public ClaimsPrincipal? AuthenticatedClaimsPrincipal { get; set; }

    #endregion Properties

}
