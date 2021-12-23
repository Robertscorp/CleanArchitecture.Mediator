namespace CleanArchitecture.Services.DependencyInjection.Validation
{

    public class ValidationException : Exception
    {

        #region - - - - - - Constructors - - - - - -

        internal ValidationException(string message) : base(message) { }

        #endregion Constructors

    }

}
