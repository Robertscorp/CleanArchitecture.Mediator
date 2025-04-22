using CleanArchitecture.Mediator;

namespace CleanArchitecture.Sample.UseCases.CreateProduct
{

    public class CreateProductValidator : IValidator<CreateProductInputPort, ICreateProductOutputPort>
    {

        #region - - - - - - Methods - - - - - -

        public async Task<bool> HandleValidationAsync(CreateProductInputPort inputPort, ICreateProductOutputPort outputPort, ServiceFactory serviceFactory, CancellationToken cancellationToken)
        {
            if (inputPort.FailValidation)
            {
                await outputPort.PresentValidationFailureAsync(cancellationToken).ConfigureAwait(false);
                return false;
            }

            return true;
        }

        #endregion Methods

    }

}
