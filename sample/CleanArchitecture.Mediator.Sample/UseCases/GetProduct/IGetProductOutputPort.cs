using CleanArchitecture.Mediator.Sample.Dtos;

namespace CleanArchitecture.Mediator.Sample.UseCases.GetProduct;

public interface IGetProductOutputPort
{

    #region - - - - - - Methods - - - - - -

    Task PresentProductAsync(ProductDto product, CancellationToken cancellationToken);

    #endregion Methods

}
