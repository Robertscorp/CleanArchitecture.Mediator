using CleanArchitecture.Mediator.Sample.Dtos;
using CleanArchitecture.Mediator.Sample.OutputPorts;
using CleanArchitecture.Mediator.Sample.UseCases.GetProduct;

namespace CleanArchitecture.Mediator.Sample.Presenters;

public class GetProductPresenter : IGetProductOutputPort, IVerificationSuccessOutputPort
{

    #region - - - - - - Methods - - - - - -

    Task IGetProductOutputPort.PresentProductAsync(ProductDto product, CancellationToken cancellationToken)
    {
        Console.WriteLine($"\t- GetProductPresenter.PresentProductAsync('{product.Name}')");
        return Task.CompletedTask;
    }

    Task IVerificationSuccessOutputPort.PresentVerificationSuccessAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"\t- GetProductPresenter.PresentVerificationSuccessAsync");
        return Task.CompletedTask;
    }

    #endregion Methods

}
