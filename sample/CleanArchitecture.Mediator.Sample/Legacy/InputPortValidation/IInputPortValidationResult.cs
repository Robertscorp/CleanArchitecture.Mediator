namespace CleanArchitecture.Mediator.Sample.Legacy.InputPortValidation;

/// <summary>
/// A validation result from an input port validator.
/// </summary>
public interface IInputPortValidationResult
{

    #region - - - - - - Properties - - - - - -

    /// <summary>
    /// Determines if a validation failure should be presented.
    /// </summary>
    bool IsValid { get; }

    #endregion Properties

}
