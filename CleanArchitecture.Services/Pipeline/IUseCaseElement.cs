namespace CleanArchitecture.Services.Pipeline
{

    public interface IUseCaseElement
    {

        #region - - - - - - Methods - - - - - -

        Task HandleAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            UseCaseElementHandleAsync nextUseCaseElementHandle,
            CancellationToken cancellationToken);

        #endregion Methods

    }

}
