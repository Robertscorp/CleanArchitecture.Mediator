namespace CleanArchitecture.Services
{

    /// <summary>
    /// A Validation Result from a Validator.
    /// </summary>
    public interface IValidationResult
    {

        #region - - - - - - Properties - - - - - -

        /// <summary>
        /// Determines if a validation failure should be presented.
        /// </summary>
        bool IsValid { get; }

        #endregion Properties

    }

}
