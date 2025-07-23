namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Represents a result from an <see cref="IInputPortValidator{TInputPort, TValidationResult}"/>.
    /// </summary>
    public interface IInputPortValidationResult
    {

        #region - - - - - - Properties - - - - - -

        /// <summary>
        /// Determines if a validation failure will be presented.
        /// </summary>
        bool IsValid { get; set; }

        #endregion Properties

    }

}
