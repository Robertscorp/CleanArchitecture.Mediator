using CleanArchitecture.Sample.Pipeline;
using CleanArchitecture.Services;

namespace CleanArchitecture.Sample.UseCases.CreateProduct
{

    public class CreateProductAuthorisationEnforcer : IUseCaseAuthorisationEnforcer<CreateProductInputPort, AuthorisationResult>
    {

        #region - - - - - - IUseCaseAuthorisationEnforcer Implementation - - - - - -

        public Task<AuthorisationResult> CheckAuthorisationAsync(CreateProductInputPort inputPort, CancellationToken cancellationToken)
            => Task.FromResult(new AuthorisationResult { IsAuthorised = !inputPort.FailAuthorisation });

        #endregion IUseCaseAuthorisationEnforcer Implementation

    }

}
