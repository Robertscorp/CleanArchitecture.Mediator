namespace CleanArchitecture.Services.Pipeline
{

    public interface IUseCaseElement
    {

        #region - - - - - - Methods - - - - - -

        Task<bool> TryOutputResultAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            CancellationToken cancellationToken);

        #endregion Methods

    }

}
