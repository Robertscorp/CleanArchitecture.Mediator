namespace CleanArchitecture.Services
{

    public interface IUseCaseInputPortValidator<TUseCaseInputPort, TValidationResult>
        where TUseCaseInputPort : IUseCaseInputPort<IValidationOutputPort<TValidationResult>>
        where TValidationResult : IValidationResult
    {

        #region - - - - - - Methods - - - - - -

        Task<TValidationResult> ValidateAsync(TUseCaseInputPort inputPort, CancellationToken cancellationToken);

        #endregion Methods

    }

}
