namespace CleanArchitecture.Sample.Legacy.BusinessRuleValidation
{

    /// <summary>
    /// A validation result from a business rule validator.
    /// </summary>
    public interface IBusinessRuleValidationResult
    {

        #region - - - - - - Properties - - - - - -

        /// <summary>
        /// Determines if a validation failure should be presented.
        /// </summary>
        bool IsValid { get; }

        #endregion Properties

    }

}
