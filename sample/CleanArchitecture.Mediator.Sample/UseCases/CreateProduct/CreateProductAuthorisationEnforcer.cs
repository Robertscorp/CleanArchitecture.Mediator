namespace CleanArchitecture.Mediator.Sample.UseCases.CreateProduct;

public class CreateProductAuthorisationEnforcer : IAuthorisationEnforcer<CreateProductInputPort, ICreateProductOutputPort>
{

    #region - - - - - - Methods - - - - - -

    public async Task<bool> HandleAuthorisationAsync(CreateProductInputPort inputPort, ICreateProductOutputPort outputPort, ServiceFactory serviceFactory, CancellationToken cancellationToken)
    {
        if (inputPort.FailAuthorisation)
        {
            await outputPort.PresentUnauthorisedAsync(cancellationToken).ConfigureAwait(false);
            return false;
        }

        return true;
    }

    #endregion Methods

}
