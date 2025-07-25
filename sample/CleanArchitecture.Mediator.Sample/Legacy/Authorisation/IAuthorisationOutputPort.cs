﻿namespace CleanArchitecture.Mediator.Sample.Legacy.Authorisation;

/// <summary>
/// An output port for when authorisation is required.
/// </summary>
/// <typeparam name="TAuthorisationFailure">The type of authorisation failure for the pipeline.</typeparam>
public interface IAuthorisationOutputPort<TAuthorisationFailure> where TAuthorisationFailure : IAuthorisationResult
{

    #region - - - - - - Methods - - - - - -

    /// <summary>
    /// Presents an authorisation failure.
    /// </summary>
    /// <param name="authorisationFailure">The authorisation failure that occurred.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
    Task PresentUnauthorisedAsync(TAuthorisationFailure authorisationFailure, CancellationToken cancellationToken);

    #endregion Methods

}
