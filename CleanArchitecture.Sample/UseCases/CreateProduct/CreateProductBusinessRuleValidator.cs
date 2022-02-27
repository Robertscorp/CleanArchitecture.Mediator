using CleanArchitecture.Mediator;
using CleanArchitecture.Sample.Pipeline;

namespace CleanArchitecture.Sample.UseCases.CreateProduct
{

    public class CreateProductBusinessRuleValidator : IUseCaseBusinessRuleValidator<CreateProductInputPort, ValidationResult>
    {

        #region - - - - - - Methods - - - - - -

        Task<ValidationResult> IUseCaseBusinessRuleValidator<CreateProductInputPort, ValidationResult>.ValidateAsync(
            CreateProductInputPort inputPort,
            CancellationToken cancellationToken)
            => Task.FromResult(new ValidationResult { IsValid = !inputPort.FailBusinessRuleValidation });

        #endregion Methods

    }

}
