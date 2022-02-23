using CleanArchitecture.Sample.Dtos;
using CleanArchitecture.Sample.UseCases.GetProduct;

namespace CleanArchitecture.Sample.Presenters
{

    public class GetProductPresenter : IGetProductOutputPort
    {

        #region - - - - - - Methods - - - - - -

        Task IGetProductOutputPort.PresentProductAsync(ProductDto product, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Get Product Presenter - Get Product '{product.Name}'");
            return Task.CompletedTask;
        }

        #endregion Methods

    }

}
