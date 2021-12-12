using CleanArchitecture.Services.Pipeline;

namespace CleanArchitecture.Sample.UseCases.CreateProduct
{

    public class CreateProductInteractor : IUseCaseInteractor<CreateProductInputPort, ICreateProductOutputPort>
    {

        #region - - - - - - IUseCaseInteractor Implementation - - - - - -

        public Task HandleAsync(CreateProductInputPort inputPort, ICreateProductOutputPort outputPort, CancellationToken cancellationToken)
            => outputPort.PresentCreatedProductAsync(new() { Name = $"Created - {DateTime.Now}" }, cancellationToken);

        #endregion IUseCaseInteractor Implementation

    }

}
