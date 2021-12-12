namespace CleanArchitecture.Services.Pipeline
{

    public interface IUseCaseInteractor<in TUseCaseInputPort, in TUseCaseOutputPort>
    {

        #region - - - - - - Methods - - - - - -

        Task HandleAsync(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            CancellationToken cancellationToken);

        #endregion Methods

    }

}
