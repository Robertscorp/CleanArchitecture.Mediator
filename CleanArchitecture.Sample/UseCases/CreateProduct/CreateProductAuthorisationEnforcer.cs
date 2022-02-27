using CleanArchitecture.Mediator;
using CleanArchitecture.Sample.Pipeline;

namespace CleanArchitecture.Sample.UseCases.CreateProduct
{

    public class CreateProductAuthorisationEnforcer : IUseCaseAuthorisationEnforcer<CreateProductInputPort, AuthorisationResult>
    {

        #region - - - - - - Methods - - - - - -

        Task<AuthorisationResult> IUseCaseAuthorisationEnforcer<CreateProductInputPort, AuthorisationResult>.CheckAuthorisationAsync(
            CreateProductInputPort inputPort,
            CancellationToken cancellationToken)
            => Task.FromResult(new AuthorisationResult { IsAuthorised = !inputPort.FailAuthorisation });

        #endregion Methods

    }

}
