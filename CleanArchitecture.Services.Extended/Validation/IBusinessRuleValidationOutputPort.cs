namespace CleanArchitecture.Services.Extended.Validation
{

    public interface IBusinessRuleValidationOutputPort<TValidationResult> where TValidationResult : IValidationResult
    {

        #region - - - - - - Methods - - - - - -

        Task PresentBusinessRuleValidationFailureAsync(TValidationResult validationResult, CancellationToken cancellationToken);

        #endregion Methods

    }

}
