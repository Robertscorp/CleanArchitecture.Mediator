namespace CleanArchitecture.Mediator.Sample.UseCases.CreateProduct;

public class CreateProductLicenceVerifier : ILicenceVerifier<CreateProductInputPort, object>
{

    #region - - - - - - Methods - - - - - -

    Task<bool> ILicenceVerifier<CreateProductInputPort, object>.IsLicencedAsync(
        CreateProductInputPort inputPort,
        out object licenceFailure,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
    {
        licenceFailure = new();

        return Task.FromResult(!inputPort.FailLicenceVerification);
    }

    #endregion Methods

}
