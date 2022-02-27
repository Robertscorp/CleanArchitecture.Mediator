using CleanArchitecture.Mediator;

namespace CleanArchitecture.Sample.UseCases.CreateProduct
{

    public class CreateProductInteractor : IUseCaseInteractor<CreateProductInputPort, ICreateProductOutputPort>
    {

        #region - - - - - - Methods - - - - - -

        Task IUseCaseInteractor<CreateProductInputPort, ICreateProductOutputPort>.HandleAsync(
            CreateProductInputPort inputPort,
            ICreateProductOutputPort outputPort,
            CancellationToken cancellationToken)
            => outputPort.PresentCreatedProductAsync(new() { Name = $"Created - {DateTime.Now}" }, cancellationToken);

        #endregion Methods

    }

}
