namespace CleanArchitecture.Services.Pipeline.Validation
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
