namespace CleanArchitecture.Mediator.Sample.UseCases.LegacyCreateProduct;

public class LegacyCreateProductInputPortValidator : IInputPortValidator<LegacyCreateProductInputPort, object>
{

    #region - - - - - - Methods - - - - - -

    public Task<bool> ValidateAsync(LegacyCreateProductInputPort inputPort, out object validationFailure, ServiceFactory serviceFactory, CancellationToken cancellationToken)
    {
        validationFailure = new();

        return Task.FromResult(!inputPort.FailInputPortValidation);
    }

    #endregion Methods

}
