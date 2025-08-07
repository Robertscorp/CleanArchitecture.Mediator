namespace CleanArchitecture.Mediator.Sample.UseCases.CreateProduct;

public class CreateProductLicenceVerifier : ILicencePolicyValidator<CreateProductInputPort, object>
{

    #region - - - - - - Methods - - - - - -

    Task<bool> ILicencePolicyValidator<CreateProductInputPort, object>.ValidateAsync(
        CreateProductInputPort inputPort,
        out object policyFailure,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
    {
        policyFailure = new();

        return Task.FromResult(!inputPort.FailLicenceVerification);
    }

    #endregion Methods

}
