using CleanArchitecture.Sample.Legacy.InputPortValidation;
using CleanArchitecture.Sample.UseCases.LegacyCreateProduct;

namespace CleanArchitecture.Sample.UseCases.CreateProduct
{

    public class LegacyCreateProductInputPortValidator : IInputPortValidator<LegacyCreateProductInputPort, InputPortValidationResult>
    {

        #region - - - - - - Methods - - - - - -

        public Task<InputPortValidationResult> ValidateAsync(LegacyCreateProductInputPort inputPort, CancellationToken cancellationToken)
            => Task.FromResult(new InputPortValidationResult() { IsValid = !inputPort.FailInputPortValidation });

        #endregion Methods

    }

}
