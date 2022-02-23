using CleanArchitecture.Sample.Pipeline;
using CleanArchitecture.Services;

namespace CleanArchitecture.Sample.UseCases.CreateProduct
{

    public class CreateProductInputPortValidator : IUseCaseInputPortValidator<CreateProductInputPort, ValidationResult>
    {

        #region - - - - - - Methods - - - - - -

        Task<ValidationResult> IUseCaseInputPortValidator<CreateProductInputPort, ValidationResult>.ValidateAsync(
            CreateProductInputPort inputPort,
            CancellationToken cancellationToken)
            => Task.FromResult(new ValidationResult { IsValid = !inputPort.FailInputPortValidation });

        #endregion Methods

    }

}
