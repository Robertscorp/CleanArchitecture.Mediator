using CleanArchitecture.Mediator.Sample.Legacy.BusinessRuleValidation;

namespace CleanArchitecture.Mediator.Sample.UseCases.LegacyCreateProduct
{

    public class LegacyCreateProductBusinessRuleValidator : IBusinessRuleValidator<LegacyCreateProductInputPort, BusinessRuleValidationResult>
    {

        #region - - - - - - Methods - - - - - -

        public Task<BusinessRuleValidationResult> ValidateAsync(LegacyCreateProductInputPort inputPort, CancellationToken cancellationToken)
            => Task.FromResult(new BusinessRuleValidationResult() { IsValid = !inputPort.FailBusinessRuleValidation });

        #endregion Methods

    }

}
