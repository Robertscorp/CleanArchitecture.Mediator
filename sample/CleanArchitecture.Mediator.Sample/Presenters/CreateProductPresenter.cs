using CleanArchitecture.Mediator.Sample.Dtos;
using CleanArchitecture.Mediator.Sample.OutputPorts;
using CleanArchitecture.Mediator.Sample.UseCases.CreateProduct;

namespace CleanArchitecture.Mediator.Sample.Presenters;

public class CreateProductPresenter : ICreateProductOutputPort, IVerificationSuccessOutputPort
{

    #region - - - - - - Properties - - - - - -

    public bool WarnOnInputPortValidationFailure { get; set; }

    #endregion Properties

    #region - - - - - - Methods - - - - - -

    Task IAuthenticationFailureOutputPort.PresentAuthenticationFailureAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("\t- CreateProductPresenter.PresentAuthenticationFailureAsync");
        return Task.CompletedTask;
    }

    Task<ContinuationBehaviour> IAuthorisationPolicyFailureOutputPort<object>.PresentAuthorisationPolicyFailureAsync(object policyFailure, CancellationToken cancellationToken)
    {
        Console.WriteLine("\t- CreateProductPresenter.PresentAuthorisationPolicyFailureAsync");
        return ContinuationBehaviour.ReturnAsync;
    }

    Task<ContinuationBehaviour> ICreateProductOutputPort.PresentCategoryDoesNotExistAsync(int categoryID, CancellationToken cancellationToken)
    {
        Console.WriteLine("\t- CreateProductPresenter.PresentCategoryDoesNotExistAsync [Warn]");
        return ContinuationBehaviour.ContinueAsync;
    }

    Task ICreateProductOutputPort.PresentCreatedProductAsync(ProductDto product, CancellationToken cancellationToken)
    {
        Console.WriteLine($"\t- CreateProductPresenter.PresentCreatedProductAsync('{product.Name}')");
        return Task.CompletedTask;
    }

    Task<ContinuationBehaviour> ICreateProductOutputPort.PresentNameMustBeUniqueAsync(string name, CancellationToken cancellationToken)
    {
        Console.WriteLine($"\t- CreateProductPresenter.PresentNameMustBeUniqueAsync [Fail]");
        return ContinuationBehaviour.ReturnAsync;
    }

    async Task<ContinuationBehaviour> IInputPortValidationFailureOutputPort<object>.PresentInputPortValidationFailureAsync(object validationFailure, CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken); // This only exists to showcase when to use the non-async ContinuationBehaviour fields.

        Console.WriteLine($"\t- CreateProductPresenter.PresentInputPortValidationFailureAsync");

        return this.WarnOnInputPortValidationFailure
            ? ContinuationBehaviour.Continue
            : ContinuationBehaviour.Return;
    }

    Task<ContinuationBehaviour> ILicencePolicyFailureOutputPort<object>.PresentLicencePolicyFailureAsync(object policyFailure, CancellationToken cancellationToken)
    {
        Console.WriteLine($"\t- CreateProductPresenter.PresentLicencePolicyFailureAsync");
        return ContinuationBehaviour.ReturnAsync;
    }

    Task IVerificationSuccessOutputPort.PresentVerificationSuccessAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("\t- CreateProductPresenter.PresentVerificationSuccessAsync");
        return Task.CompletedTask;
    }

    #endregion Methods

}
