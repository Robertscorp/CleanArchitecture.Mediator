namespace CleanArchitecture.Services.Pipeline.Authorisation
{

    public interface IAuthorisationOutputPort<TAuthorisationFailure>
    {

        #region - - - - - - Methods - - - - - -

        Task PresentUnauthorisedAsync(TAuthorisationFailure authorisationFailure, CancellationToken cancellationToken);

        #endregion Methods

    }

}
