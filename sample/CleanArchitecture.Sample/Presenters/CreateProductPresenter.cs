using CleanArchitecture.Mediator;
using CleanArchitecture.Sample.Dtos;
using CleanArchitecture.Sample.OutputPorts;
using CleanArchitecture.Sample.UseCases.CreateProduct;

namespace CleanArchitecture.Sample.Presenters
{

    public class CreateProductPresenter : ICreateProductOutputPort, IVerificationSuccessOutputPort
    {

        #region - - - - - - Methods - - - - - -

        Task IAuthenticationOutputPort.PresentUnauthenticatedAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("\t- CreateProductPresenter.PresentUnauthenticatedAsync");
            return Task.CompletedTask;
        }

        Task ICreateProductOutputPort.PresentCreatedProductAsync(ProductDto product, CancellationToken cancellationToken)
        {
            Console.WriteLine($"\t- CreateProductPresenter.PresentCreatedProductAsync('{product.Name}')");
            return Task.CompletedTask;
        }

        Task ICreateProductOutputPort.PresentUnauthorisedAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("\t- CreateProductPresenter.PresentUnauthorisedAsync");
            return Task.CompletedTask;
        }

        Task ICreateProductOutputPort.PresentValidationFailureAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("\t- CreateProductPresenter.PresentValidationFailureAsync");
            return Task.CompletedTask;
        }

        Task IVerificationSuccessOutputPort.PresentVerificationSuccessAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("\t- CreateProductPresenter.PresentVerificationSuccessAsync");
            return Task.CompletedTask;
        }

        #endregion Methods

    }

}
