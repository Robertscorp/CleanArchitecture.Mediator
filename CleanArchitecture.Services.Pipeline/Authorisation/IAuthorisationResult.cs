namespace CleanArchitecture.Services.Pipeline.Authorisation
{

    public interface IAuthorisationResult
    {

        #region - - - - - - Properties - - - - - -

        bool IsAuthorised { get; }

        #endregion Properties

    }

}
