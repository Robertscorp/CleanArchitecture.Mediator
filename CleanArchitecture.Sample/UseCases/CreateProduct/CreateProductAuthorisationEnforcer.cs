using CleanArchitecture.Mediator;
using CleanArchitecture.Sample.Pipelines;

namespace CleanArchitecture.Sample.UseCases.CreateProduct
{

    public class CreateProductAuthorisationEnforcer : IAuthorisationEnforcer<CreateProductInputPort, AuthorisationResult>
    {

        #region - - - - - - Methods - - - - - -

        Task<AuthorisationResult> IAuthorisationEnforcer<CreateProductInputPort, AuthorisationResult>.CheckAuthorisationAsync(
            CreateProductInputPort inputPort,
            CancellationToken cancellationToken)
            => Task.FromResult(new AuthorisationResult { IsAuthorised = !inputPort.FailAuthorisation });

        #endregion Methods

    }

}
