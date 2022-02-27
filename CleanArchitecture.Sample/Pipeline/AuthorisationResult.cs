using CleanArchitecture.Mediator;

namespace CleanArchitecture.Sample.Pipeline
{

    public class AuthorisationResult : IAuthorisationResult
    {

        #region - - - - - - Properties - - - - - -

        public bool IsAuthorised { get; set; }

        #endregion Properties

    }

}
