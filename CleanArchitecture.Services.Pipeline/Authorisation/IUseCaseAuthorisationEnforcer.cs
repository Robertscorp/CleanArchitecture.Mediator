namespace CleanArchitecture.Services.Pipeline.Authorisation
{

    public interface IUseCaseAuthorisationEnforcer<TUseCaseInputPort, TAuthorisationResult>
        where TUseCaseInputPort : IUseCaseInputPort<IAuthorisationOutputPort<TAuthorisationResult>>
        where TAuthorisationResult : IAuthorisationResult
    {

        #region - - - - - - Methods - - - - - -

        Task<TAuthorisationResult> CheckAuthorisationAsync(TUseCaseInputPort inputPort, CancellationToken cancellationToken);

        #endregion Methods

    }

}
