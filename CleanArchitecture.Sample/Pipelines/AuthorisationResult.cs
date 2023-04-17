using CleanArchitecture.Mediator;

namespace CleanArchitecture.Sample.Pipelines
{

    public class AuthorisationResult : IAuthorisationResult
    {

        #region - - - - - - Properties - - - - - -

        public bool IsAuthorised { get; set; }

        #endregion Properties

    }

}
