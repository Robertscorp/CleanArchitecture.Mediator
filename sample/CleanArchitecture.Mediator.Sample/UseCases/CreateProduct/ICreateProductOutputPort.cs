using CleanArchitecture.Mediator.Sample.Dtos;

namespace CleanArchitecture.Mediator.Sample.UseCases.CreateProduct
{

    public interface ICreateProductOutputPort : IAuthenticationOutputPort
    {

        #region - - - - - - Methods - - - - - -

        Task PresentCreatedProductAsync(ProductDto product, CancellationToken cancellationToken);

        Task PresentUnauthorisedAsync(CancellationToken cancellationToken);

        Task PresentValidationFailureAsync(CancellationToken cancellationToken);

        #endregion Methods

    }

}
