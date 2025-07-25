﻿namespace CleanArchitecture.Mediator.Sample.Legacy.Authorisation;

/// <summary>
/// An authorisation result from an authorisation enforcer.
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
