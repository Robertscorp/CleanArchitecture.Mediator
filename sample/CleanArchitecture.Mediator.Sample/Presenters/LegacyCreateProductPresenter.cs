using CleanArchitecture.Mediator.Sample.Dtos;
using CleanArchitecture.Mediator.Sample.Legacy.BusinessRuleValidation;
using CleanArchitecture.Mediator.Sample.UseCases.LegacyCreateProduct;

namespace CleanArchitecture.Mediator.Sample.Presenters;

public class LegacyCreateProductPresenter : ILegacyCreateProductOutputPort
{

    #region - - - - - - Methods - - - - - -

    Task IAuthenticationOutputPort.PresentUnauthenticatedAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("\t- LegacyCreateProductPresenter.PresentUnauthenticatedAsync");
        return Task.CompletedTask;
    }

    Task IAuthorisationOutputPort<object>.PresentAuthorisationFailureAsync(object authorisationFailure, CancellationToken cancellationToken)
    {
        Console.WriteLine("\t- LegacyCreateProductPresenter.PresentAuthorisationFailureAsync");
        return Task.CompletedTask;
    }

    Task IBusinessRuleValidationOutputPort<BusinessRuleValidationResult>.PresentValidationFailureAsync(BusinessRuleValidationResult validationFailure, CancellationToken cancellationToken)
    {
        Console.WriteLine("\t- LegacyCreateProductPresenter.PresentBusinessRuleFailureAsync");
        return Task.CompletedTask;
    }

    Task IInputPortValidationOutputPort<object>.PresentInputPortValidationFailureAsync(object validationFailure, CancellationToken cancellationToken)
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
