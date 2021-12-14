namespace CleanArchitecture.Services
{

    public interface IUseCaseBusinessRuleValidator<TUseCaseInputPort, TValidationResult>
        where TUseCaseInputPort : IUseCaseInputPort<IBusinessRuleValidationOutputPort<TValidationResult>>
        where TValidationResult : IValidationResult
    {

        #region - - - - - - Methods - - - - - -

        Task<TValidationResult> ValidateAsync(TUseCaseInputPort inputPort, CancellationToken cancellationToken);

        #endregion Methods

    }

}
