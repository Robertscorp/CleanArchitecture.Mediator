namespace CleanArchitecture.Services
{

    public interface IAuthenticationOutputPort
    {

        #region - - - - - - Methods - - - - - -

        Task PresentUnauthenticatedAsync(CancellationToken cancellationToken);

        #endregion Methods

    }

}
