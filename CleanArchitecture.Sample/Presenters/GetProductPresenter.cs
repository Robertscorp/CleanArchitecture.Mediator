using CleanArchitecture.Sample.Dtos;
using CleanArchitecture.Sample.UseCases.GetProduct;

namespace CleanArchitecture.Sample.Presenters
{

    public class GetProductPresenter : IGetProductOutputPort
    {

        #region - - - - - - IGetProductOutputPort Implementation - - - - - -

        public Task PresentProductAsync(ProductDto product, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Get Product Presenter - Get Product '{product.Name}'");
            return Task.CompletedTask;
        }

        #endregion IGetProductOutputPort Implementation

    }

}
