using CleanArchitecture.Mediator;
using CleanArchitecture.Sample.Dtos;

namespace CleanArchitecture.Sample.UseCases.CreateProduct
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
