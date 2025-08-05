using CleanArchitecture.Mediator.Sample.Dtos;

namespace CleanArchitecture.Mediator.Sample.UseCases.CreateProduct;

public interface ICreateProductOutputPort : IAuthenticationOutputPort, IAuthorisationOutputPort<object>, ILicenceEnforcementOutputPort<object>
{

    #region - - - - - - Methods - - - - - -

    Task PresentCategoryDoesNotExistAsync(int categoryID, CancellationToken cancellationToken);

    Task PresentCreatedProductAsync(ProductDto product, CancellationToken cancellationToken);

    Task PresentNameMustBeUniqueAsync(string name, CancellationToken cancellationToken);

    #endregion Methods

}
