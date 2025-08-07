using CleanArchitecture.Mediator.Sample.Dtos;
using CleanArchitecture.Mediator.Sample.OutputPorts;
using CleanArchitecture.Mediator.Sample.UseCases.CreateProduct;

namespace CleanArchitecture.Mediator.Sample.Presenters;

public class CreateProductPresenter : ICreateProductOutputPort, IVerificationSuccessOutputPort
{

    #region - - - - - - Methods - - - - - -

    Task IAuthenticationOutputPort.PresentAuthenticationFailureAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("\t- CreateProductPresenter.PresentAuthenticationFailureAsync");
        return Task.CompletedTask;
    }

    Task IAuthorisationPolicyFailureOutputPort<object>.PresentAuthorisationPolicyFailureAsync(object policyFailure, CancellationToken cancellationToken)
    {
        Console.WriteLine("\t- CreateProductPresenter.PresentAuthorisationPolicyFailureAsync");
        return Task.CompletedTask;
    }

    Task ICreateProductOutputPort.PresentCategoryDoesNotExistAsync(int categoryID, CancellationToken cancellationToken)
    {
        Console.WriteLine("\t- CreateProductPresenter.PresentCategoryDoesNotExistAsync");
        return Task.CompletedTask;
    }

    Task ICreateProductOutputPort.PresentCreatedProductAsync(ProductDto product, CancellationToken cancellationToken)
    {
        Console.WriteLine($"\t- CreateProductPresenter.PresentCreatedProductAsync('{product.Name}')");
        return Task.CompletedTask;
    }

    Task ICreateProductOutputPort.PresentNameMustBeUniqueAsync(string name, CancellationToken cancellationToken)
    {
        Console.WriteLine("\t- CreateProductPresenter.PresentNameMustBeUniqueAsync");
        return Task.CompletedTask;
    }

    Task IInputPortValidationOutputPort<object>.PresentInputPortValidationFailureAsync(object validationFailure, CancellationToken cancellationToken)
    {
        Console.WriteLine($"\t- CreateProductPresenter.PresentInputPortValidationFailureAsync");
        return Task.CompletedTask;
    }

    Task ILicencePolicyFailureOutputPort<object>.PresentLicencePolicyFailureAsync(object policyFailure, CancellationToken cancellationToken)
    {
        Console.WriteLine($"\t- CreateProductPresenter.PresentLicencePolicyFailureAsync");
        return Task.CompletedTask;
    }

    Task IVerificationSuccessOutputPort.PresentVerificationSuccessAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("\t- CreateProductPresenter.PresentVerificationSuccessAsync");
        return Task.CompletedTask;
    }

    #endregion Methods

}
