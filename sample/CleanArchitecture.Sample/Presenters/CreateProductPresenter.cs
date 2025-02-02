using CleanArchitecture.Mediator;
using CleanArchitecture.Sample.Dtos;
using CleanArchitecture.Sample.OutputPorts;
using CleanArchitecture.Sample.Pipelines;
using CleanArchitecture.Sample.UseCases.CreateProduct;

namespace CleanArchitecture.Sample.Presenters
{

    public class CreateProductPresenter : ICreateProductOutputPort, IVerificationSuccessOutputPort
    {

        #region - - - - - - Methods - - - - - -

        Task IAuthenticationOutputPort.PresentUnauthenticatedAsync(CancellationToken cancellationToken)
        {
            Console.Write(" CreateProductPresenter.PresentUnauthenticatedAsync");
            return Task.CompletedTask;
        }

        Task ICreateProductOutputPort.PresentCreatedProductAsync(ProductDto product, CancellationToken cancellationToken)
        {
            Console.Write($" CreateProductPresenter.PresentCreatedProductAsync('{product.Name}')");
            return Task.CompletedTask;
        }

        Task ICreateProductOutputPort.PresentUnauthorisedAsync(CancellationToken cancellationToken)
        {
            Console.Write(" CreateProductPresenter.PresentUnauthorisedAsync");
            return Task.CompletedTask;
        }

        Task IValidationOutputPort<ValidationResult>.PresentValidationFailureAsync(ValidationResult validationFailure, CancellationToken cancellationToken)
        {
            Console.Write(" CreateProductPresenter.PresentValidationFailureAsync");
            return Task.CompletedTask;
        }

        Task IVerificationSuccessOutputPort.PresentVerificationSuccessAsync(CancellationToken cancellationToken)
        {
            Console.Write(" CreateProductPresenter.PresentVerificationSuccessAsync");
            return Task.CompletedTask;
        }

        #endregion Methods

    }

}
