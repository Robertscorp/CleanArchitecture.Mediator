using CleanArchitecture.Services;

namespace CleanArchitecture.Sample.Pipeline
{

    public class ValidationResult : IValidationResult
    {

        #region - - - - - - IValidationResult Implementation - - - - - -

        public bool IsValid { get; set; }

        #endregion IValidationResult Implementation

    }

}
