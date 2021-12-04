namespace CleanArchitecture.Services.Pipeline
{

    public interface IUseCaseInvoker
    {

        #region - - - - - - Methods - - - - - -

        Task InvokeUseCaseAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            CancellationToken cancellationToken);

        #endregion Methods

    }

}
