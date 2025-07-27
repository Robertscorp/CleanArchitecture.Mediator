using CleanArchitecture.Mediator.Sample.Dtos;

namespace CleanArchitecture.Mediator.Sample.UseCases.CreateProduct;

public interface ICreateProductOutputPort : IAuthenticationOutputPort, IAuthorisationOutputPort<object>
{

    #region - - - - - - Methods - - - - - -

    Task PresentCreatedProductAsync(ProductDto product, CancellationToken cancellationToken);

    Task PresentValidationFailureAsync(CancellationToken cancellationToken);

    #endregion Methods

}
