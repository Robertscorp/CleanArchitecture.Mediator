using CleanArchitecture.Services.Extended.Validation;

namespace CleanArchitecture.Services.Extended.Pipeline
{

    public interface IUseCaseBusinessRuleValidator<TUseCaseInputPort, TValidationResult> where TValidationResult : IValidationResult
    {

        #region - - - - - - Methods - - - - - -

        Task<TValidationResult> ValidateAsync(TUseCaseInputPort inputPort, CancellationToken cancellationToken);

        #endregion Methods

    }

}
