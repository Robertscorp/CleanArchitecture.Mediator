using System.Security.Principal;

namespace CleanArchitecture.Mediator.Sample.Framework.Infrastructure;

public class PrincipalStore : IPrincipalAccessor
{

    #region - - - - - - Properties - - - - - -

    public IPrincipal? Principal { get; set; }

    #endregion Properties

}
