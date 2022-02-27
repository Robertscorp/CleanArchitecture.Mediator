namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// An authorisation result from an Authorisation Enforcer.
    /// </summary>
    public interface IAuthorisationResult
    {

        #region - - - - - - Properties - - - - - -

        /// <summary>
        /// Determines if an authorisation failure should be presented.
        /// </summary>
        bool IsAuthorised { get; }

        #endregion Properties

    }

}
