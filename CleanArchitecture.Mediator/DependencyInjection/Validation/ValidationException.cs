using System;

namespace CleanArchitecture.Mediator.DependencyInjection.Validation
{

    /// <summary>
    /// An exception thrown after Registration when an issue is detected.
    /// </summary>
    public class ValidationException : Exception
    {

        #region - - - - - - Constructors - - - - - -

        internal ValidationException(string message) : base(message) { }

        #endregion Constructors

    }

}
