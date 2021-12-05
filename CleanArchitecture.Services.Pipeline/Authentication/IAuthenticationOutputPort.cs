namespace CleanArchitecture.Services.Pipeline.Authentication
{

    public interface IAuthenticationOutputPort
    {

        #region - - - - - - Methods - - - - - -

        Task PresentUnauthenticatedAsync(CancellationToken cancellationToken);

        #endregion Methods

    }

}
