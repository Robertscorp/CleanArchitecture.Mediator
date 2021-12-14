using CleanArchitecture.Sample.Pipeline;
using CleanArchitecture.Services;

namespace CleanArchitecture.Sample.UseCases.CreateProduct
{

    public class CreateProductInputPortValidator : IUseCaseInputPortValidator<CreateProductInputPort, ValidationResult>
    {

        #region - - - - - - IUseCaseInputPortValidator Implementation - - - - - -

        public Task<ValidationResult> ValidateAsync(CreateProductInputPort inputPort, CancellationToken cancellationToken)
            => Task.FromResult(new ValidationResult { IsValid = !inputPort.FailInputPortValidation });

        #endregion IUseCaseInputPortValidator Implementation

    }

}
