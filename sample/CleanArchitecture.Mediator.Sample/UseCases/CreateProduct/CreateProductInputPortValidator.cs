namespace CleanArchitecture.Mediator.Sample.UseCases.CreateProduct;

internal class CreateProductInputPortValidator : IInputPortValidator<CreateProductInputPort, object>
{

    #region - - - - - - Methods - - - - - -

    Task<bool> IInputPortValidator<CreateProductInputPort, object>.ValidateAsync(
        CreateProductInputPort inputPort,
        out object validationFailure,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
    {
        validationFailure = new();

        return Task.FromResult(!inputPort.FailInputPortValidation);
    }

    #endregion Methods

}
