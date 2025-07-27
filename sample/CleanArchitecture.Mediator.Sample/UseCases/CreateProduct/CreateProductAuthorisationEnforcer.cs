namespace CleanArchitecture.Mediator.Sample.UseCases.CreateProduct;

public class CreateProductAuthorisationEnforcer : IAuthorisationEnforcer<CreateProductInputPort, object>
{

    #region - - - - - - Methods - - - - - -

    public Task<bool> IsAuthorisedAsync(CreateProductInputPort inputPort, out object authorisationFailure, ServiceFactory serviceFactory, CancellationToken cancellationToken)
    {
        authorisationFailure = new();

        return Task.FromResult(!inputPort.FailAuthorisation);
    }


    #endregion Methods

}
