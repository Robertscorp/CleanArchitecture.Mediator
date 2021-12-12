using CleanArchitecture.Sample.Pipeline;
using CleanArchitecture.Services.Pipeline.Validation;

namespace CleanArchitecture.Sample.UseCases.CreateProduct
{

    public class CreateProductBusinessRuleValidator : IUseCaseBusinessRuleValidator<CreateProductInputPort, ValidationResult>
    {

        #region - - - - - - IUseCaseBusinessRuleValidator Implementation - - - - - -

        public Task<ValidationResult> ValidateAsync(CreateProductInputPort inputPort, CancellationToken cancellationToken)
            => Task.FromResult(new ValidationResult { IsValid = !inputPort.FailBusinessRuleValidation });

        #endregion IUseCaseBusinessRuleValidator Implementation

    }

}
