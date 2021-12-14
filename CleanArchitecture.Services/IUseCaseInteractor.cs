namespace CleanArchitecture.Services
{

    public interface IUseCaseInteractor<TUseCaseInputPort, TUseCaseOutputPort>
    {

        #region - - - - - - Methods - - - - - -

        Task HandleAsync(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            CancellationToken cancellationToken);

        #endregion Methods

    }

}
