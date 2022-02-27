using CleanArchitecture.Mediator;

namespace CleanArchitecture.Sample.Pipeline
{

    public class ValidationResult : IValidationResult
    {

        #region - - - - - - Properties - - - - - -

        public bool IsValid { get; set; }

        #endregion Properties

    }

}
