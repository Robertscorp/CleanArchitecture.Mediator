namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// A validation result from a validator.
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
