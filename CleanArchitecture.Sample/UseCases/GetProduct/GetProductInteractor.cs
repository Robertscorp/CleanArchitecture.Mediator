using CleanArchitecture.Mediator;
using CleanArchitecture.Sample.Dtos;

namespace CleanArchitecture.Sample.UseCases.GetProduct
{

    public class GetProductInteractor : IInteractor<GetProductInputPort, IGetProductOutputPort>
    {

        #region - - - - - - Methods - - - - - -

        Task IInteractor<GetProductInputPort, IGetProductOutputPort>.HandleAsync(
            GetProductInputPort inputPort,
            IGetProductOutputPort outputPort,
            CancellationToken cancellationToken)
            => outputPort.PresentProductAsync(new ProductDto { Name = "Hat" }, cancellationToken);

        #endregion Methods

    }

}
