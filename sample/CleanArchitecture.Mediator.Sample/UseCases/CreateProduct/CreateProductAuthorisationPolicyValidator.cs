namespace CleanArchitecture.Mediator.Sample.UseCases.CreateProduct;

public class CreateProductAuthorisationPolicyValidator : IAuthorisationPolicyValidator<CreateProductInputPort, object>
{

    #region - - - - - - Methods - - - - - -

    Task<bool> IAuthorisationPolicyValidator<CreateProductInputPort, object>.ValidateAsync(
        CreateProductInputPort inputPort,
        out object policyFailure,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
    {
        policyFailure = new();

        return Task.FromResult(!inputPort.FailAuthorisation);
    }

    #endregion Methods

}
