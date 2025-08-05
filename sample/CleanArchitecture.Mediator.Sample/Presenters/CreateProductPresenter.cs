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

    Task IAuthorisationOutputPort<object>.PresentAuthorisationFailureAsync(object authorisationFailure, CancellationToken cancellationToken)
    {
        Console.WriteLine("\t- CreateProductPresenter.PresentAuthorisationFailureAsync");
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

    Task ILicenceEnforcementOutputPort<object>.PresentLicenceFailureAsync(object licenceFailure, CancellationToken cancellationToken)
    {
        Console.WriteLine($"\t- CreateProductPresenter.PresentLicenceFailureAsync");
        return Task.CompletedTask;
    }

    Task IVerificationSuccessOutputPort.PresentVerificationSuccessAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("\t- CreateProductPresenter.PresentVerificationSuccessAsync");
        return Task.CompletedTask;
    }

    #endregion Methods

}
