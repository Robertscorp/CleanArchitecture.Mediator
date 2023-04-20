using CleanArchitecture.Sample.Dtos;
using CleanArchitecture.Sample.OutputPorts;
using CleanArchitecture.Sample.UseCases.GetProduct;

namespace CleanArchitecture.Sample.Presenters
{

    public class GetProductPresenter : IGetProductOutputPort, IVerificationSuccessOutputPort
    {

        #region - - - - - - Methods - - - - - -

        Task IGetProductOutputPort.PresentProductAsync(ProductDto product, CancellationToken cancellationToken)
        {
            Console.Write($" GetProductPresenter.PresentProductAsync('{product.Name}')");
            return Task.CompletedTask;
        }

        Task IVerificationSuccessOutputPort.PresentVerificationSuccessAsync(CancellationToken cancellationToken)
        {
            Console.Write($" GetProductPresenter.PresentVerificationSuccessAsync");
            return Task.CompletedTask;
        }

        #endregion Methods

    }

}
