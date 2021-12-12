using CleanArchitecture.Services.Pipeline.Validation;

namespace CleanArchitecture.Sample.Pipeline
{

    public class ValidationResult : IValidationResult
    {

        #region - - - - - - IValidationResult Implementation - - - - - -

        public bool IsValid { get; set; }


        #endregion IValidationResult Implementation

    }

}
