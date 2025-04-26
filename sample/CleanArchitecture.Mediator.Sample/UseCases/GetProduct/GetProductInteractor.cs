using CleanArchitecture.Mediator.Sample.Dtos;

namespace CleanArchitecture.Mediator.Sample.UseCases.GetProduct
{

    public class GetProductInteractor : IInteractor<GetProductInputPort, IGetProductOutputPort>
    {

        #region - - - - - - Methods - - - - - -

        Task IInteractor<GetProductInputPort, IGetProductOutputPort>.HandleAsync(
            GetProductInputPort inputPort,
            IGetProductOutputPort outputPort,
            ServiceFactory serviceFactory,
            CancellationToken cancellationToken)
            => outputPort.PresentProductAsync(new ProductDto { Name = "Hat" }, cancellationToken);

        #endregion Methods

    }

}
