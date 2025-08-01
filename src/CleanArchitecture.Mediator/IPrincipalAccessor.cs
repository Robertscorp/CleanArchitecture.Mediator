using System.Security.Principal;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Provides access to the currently authenticated principal.
    /// </summary>
    public interface IPrincipalAccessor
    {

        #region - - - - - - Properties - - - - - -

        /// <summary>
        /// The currently authenticated principal.
        /// </summary>
        IPrincipal Principal { get; }

        #endregion Properties

    }

}
