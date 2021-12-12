using CleanArchitecture.Sample.Dtos;
using CleanArchitecture.Sample.Pipeline;
using CleanArchitecture.Sample.UseCases.CreateProduct;

namespace CleanArchitecture.Sample.Presenters
{

    public class CreateProductPresenter : ICreateProductOutputPort
    {

        #region - - - - - - ICreateProductOutputPort Implementation - - - - - -

        public Task PresentBusinessRuleValidationFailureAsync(ValidationResult validationFailure, CancellationToken cancellationToken)
        {
            Console.WriteLine("Create Product Presenter - Business Rule Validation Failure");
            return Task.CompletedTask;
        }

        public Task PresentCreatedProductAsync(ProductDto product, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Create Product Presenter - Product Created - '{product.Name}'");
            return Task.CompletedTask;
        }

        public Task PresentUnauthenticatedAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Create Product Presenter - Unauthenticated");
            return Task.CompletedTask;
        }

        public Task PresentUnauthorisedAsync(AuthorisationResult authorisationFailure, CancellationToken cancellationToken)
        {
            Console.WriteLine("Create Product Presenter - Unauthorised");
            return Task.CompletedTask;
        }

        public Task PresentValidationFailureAsync(ValidationResult validationFailure, CancellationToken cancellationToken)
        {
            Console.WriteLine("Create Product Presenter - Input Port Validation Failure");
            return Task.CompletedTask;
        }

        #endregion ICreateProductOutputPort Implementation

    }

}
