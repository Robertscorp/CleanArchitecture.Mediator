using CleanArchitecture.Sample.Dtos;
using CleanArchitecture.Services;

namespace CleanArchitecture.Sample.UseCases.GetProduct
{

    public class GetProductInteractor : IUseCaseInteractor<GetProductInputPort, IGetProductOutputPort>
    {

        #region - - - - - - Methods - - - - - -

        Task IUseCaseInteractor<GetProductInputPort, IGetProductOutputPort>.HandleAsync(
            GetProductInputPort inputPort,
            IGetProductOutputPort outputPort,
            CancellationToken cancellationToken)
            => outputPort.PresentProductAsync(new ProductDto { Name = "Hat" }, cancellationToken);

        #endregion Methods

    }

}
