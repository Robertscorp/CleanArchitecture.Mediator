namespace CleanArchitecture.Mediator.Sample.UseCases.LegacyCreateProduct;

public class LegacyCreateProductAuthorisationEnforcer : IAuthorisationEnforcer<LegacyCreateProductInputPort, object>
{

    #region - - - - - - Methods - - - - - -

    Task<bool> IAuthorisationEnforcer<LegacyCreateProductInputPort, object>.IsAuthorisedAsync(LegacyCreateProductInputPort inputPort, out object authorisationFailure, ServiceFactory serviceFactory, CancellationToken cancellationToken)
    {
        authorisationFailure = new();

        return Task.FromResult(!inputPort.FailAuthorisation);
    }

    #endregion Methods

}
