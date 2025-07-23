using CleanArchitecture.Mediator.Sample.Services;

namespace CleanArchitecture.Mediator.Sample.UseCases.LegacyCreateProduct;

public class LegacyCreateProductInputPortValidator : IInputPortValidator<LegacyCreateProductInputPort, InputPortValidationResult>
{

    #region - - - - - - Methods - - - - - -

    public Task<InputPortValidationResult> ValidateAsync(LegacyCreateProductInputPort inputPort, ServiceFactory serviceFactory, CancellationToken cancellationToken)
        => Task.FromResult(new InputPortValidationResult() { IsValid = !inputPort.FailInputPortValidation });

    #endregion Methods

}
