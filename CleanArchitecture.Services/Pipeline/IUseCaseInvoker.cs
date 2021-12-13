namespace CleanArchitecture.Services.Pipeline
{

    public interface IUseCaseInvoker
    {

        #region - - - - - - Methods - - - - - -

        Task InvokeUseCaseAsync<TUseCaseOutputPort>(
            IUseCaseInputPort<TUseCaseOutputPort> inputPort,
            TUseCaseOutputPort outputPort,
            CancellationToken cancellationToken);

        #endregion Methods

    }

}
