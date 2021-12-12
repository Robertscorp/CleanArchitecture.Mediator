using CleanArchitecture.Sample.Dtos;

namespace CleanArchitecture.Sample.UseCases.GetProduct
{

    public interface IGetProductOutputPort
    {

        #region - - - - - - Methods - - - - - -

        Task PresentProductAsync(ProductDto product, CancellationToken cancellationToken);

        #endregion Methods

    }

}
