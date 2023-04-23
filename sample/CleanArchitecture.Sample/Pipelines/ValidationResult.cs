using CleanArchitecture.Mediator;

namespace CleanArchitecture.Sample.Pipelines
{

    public class ValidationResult : IValidationResult
    {

        #region - - - - - - Properties - - - - - -

        public bool IsValid { get; set; }

        #endregion Properties

    }

}
