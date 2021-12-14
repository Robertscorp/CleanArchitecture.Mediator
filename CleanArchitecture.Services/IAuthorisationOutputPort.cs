namespace CleanArchitecture.Services
{

    public interface IAuthorisationOutputPort<TAuthorisationFailure>
    {

        #region - - - - - - Methods - - - - - -

        Task PresentUnauthorisedAsync(TAuthorisationFailure authorisationFailure, CancellationToken cancellationToken);

        #endregion Methods

    }

}
