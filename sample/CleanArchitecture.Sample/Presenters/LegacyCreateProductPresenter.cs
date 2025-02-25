using CleanArchitecture.Mediator;
using CleanArchitecture.Sample.Dtos;
using CleanArchitecture.Sample.Legacy.Authorisation;
using CleanArchitecture.Sample.Legacy.BusinessRuleValidation;
using CleanArchitecture.Sample.Legacy.InputPortValidation;
using CleanArchitecture.Sample.UseCases.LegacyCreateProduct;

namespace CleanArchitecture.Sample.Presenters
{

    public class LegacyCreateProductPresenter : ILegacyCreateProductOutputPort
    {

        #region - - - - - - Methods - - - - - -

        Task IAuthenticationOutputPort.PresentUnauthenticatedAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("\t- LegacyCreateProductPresenter.PresentUnauthenticatedAsync");
            return Task.CompletedTask;
        }

        Task IAuthorisationOutputPort<AuthorisationResult>.PresentUnauthorisedAsync(AuthorisationResult authorisationFailure, CancellationToken cancellationToken)
        {
            Console.WriteLine("\t- LegacyCreateProductPresenter.PresentUnauthorisedAsync");
            return Task.CompletedTask;
        }

        Task IBusinessRuleValidationOutputPort<BusinessRuleValidationResult>.PresentValidationFailureAsync(BusinessRuleValidationResult validationFailure, CancellationToken cancellationToken)
        {
            Console.WriteLine("\t- LegacyCreateProductPresenter.PresentBusinessRuleFailureAsync");
            return Task.CompletedTask;
        }

        Task IInputPortValidationOutputPort<InputPortValidationResult>.PresentValidationFailureAsync(InputPortValidationResult validationFailure, CancellationToken cancellationToken)
        {
            Console.WriteLine("\t- LegacyCreateProductPresenter.PresentInputPortValidationFailureAsync");
            return Task.CompletedTask;
        }

        Task ILegacyCreateProductOutputPort.PresentCreatedProductAsync(ProductDto product, CancellationToken cancellationToken)
        {
            Console.WriteLine($"\t- LegacyCreateProductPresenter.PresentCreatedProductAsync('{product.Name}')");
            return Task.CompletedTask;
        }

        #endregion Methods

    }

}
