namespace CleanArchitecture.Mediator.Sample.Application.Services.Validation;

public class SampleInputPortValidationFailure(List<SampleInputPortValidationFailure.ValidationError> errors)
{

    #region - - - - - - Properties - - - - - -

    public List<ValidationError> Errors { get; } = errors;

    #endregion Properties

    #region - - - - - - Nested Classes - - - - - -

    public class ValidationError(string propertyName, string error)
    {

        #region - - - - - - Properties - - - - - -

        public string Error { get; } = error;

        public string PropertyName { get; } = propertyName;

        #endregion Properties

    }

    #endregion Nested Classes

}
