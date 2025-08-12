using CleanArchitecture.Mediator.Sample.Dtos;

namespace CleanArchitecture.Mediator.Sample.UseCases.CreateProduct;

public interface ICreateProductOutputPort :
    IAuthenticationFailureOutputPort,
    IAuthorisationPolicyFailureOutputPort<object>,
    IInputPortValidationFailureOutputPort<object>,
    ILicencePolicyFailureOutputPort<object>
{

    #region - - - - - - Methods - - - - - -

    Task<ContinuationBehaviour> PresentCategoryDoesNotExistAsync(int categoryID, CancellationToken cancellationToken);

    Task PresentCreatedProductAsync(ProductDto product, CancellationToken cancellationToken);

    Task<ContinuationBehaviour> PresentNameMustBeUniqueAsync(string name, CancellationToken cancellationToken);

    #endregion Methods

}
