using CleanArchitecture.Mediator;
using CleanArchitecture.Sample.Dtos;
using CleanArchitecture.Sample.Pipeline;
using CleanArchitecture.Sample.UseCases.CreateProduct;

namespace CleanArchitecture.Sample.Presenters
{

    public class CreateProductPresenter : ICreateProductOutputPort
    {

        #region - - - - - - Methods - - - - - -

        Task IBusinessRuleValidationOutputPort<ValidationResult>.PresentBusinessRuleValidationFailureAsync(ValidationResult validationFailure, CancellationToken cancellationToken)
        {
            Console.WriteLine("Create Product Presenter - Business Rule Validation Failure");
            return Task.CompletedTask;
        }

        Task ICreateProductOutputPort.PresentCreatedProductAsync(ProductDto product, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Create Product Presenter - Product Created - '{product.Name}'");
            return Task.CompletedTask;
        }

        Task IAuthenticationOutputPort.PresentUnauthenticatedAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Create Product Presenter - Unauthenticated");
            return Task.CompletedTask;
        }

        Task IAuthorisationOutputPort<AuthorisationResult>.PresentUnauthorisedAsync(AuthorisationResult authorisationFailure, CancellationToken cancellationToken)
        {
            Console.WriteLine("Create Product Presenter - Unauthorised");
            return Task.CompletedTask;
        }

        Task IValidationOutputPort<ValidationResult>.PresentValidationFailureAsync(ValidationResult validationFailure, CancellationToken cancellationToken)
        {
            Console.WriteLine("Create Product Presenter - Input Port Validation Failure");
            return Task.CompletedTask;
        }

        #endregion Methods

    }

}
