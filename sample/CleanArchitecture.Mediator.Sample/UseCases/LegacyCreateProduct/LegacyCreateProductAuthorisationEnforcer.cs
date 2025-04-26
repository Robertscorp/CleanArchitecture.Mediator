using CleanArchitecture.Mediator.Sample.Legacy.Authorisation;

namespace CleanArchitecture.Mediator.Sample.UseCases.LegacyCreateProduct
{

    public class LegacyCreateProductAuthorisationEnforcer : Legacy.Authorisation.IAuthorisationEnforcer<LegacyCreateProductInputPort, AuthorisationResult>
    {

        #region - - - - - - Methods - - - - - -

        public Task<AuthorisationResult> CheckAuthorisationAsync(LegacyCreateProductInputPort inputPort, CancellationToken cancellationToken)
            => Task.FromResult(new AuthorisationResult() { IsAuthorised = !inputPort.FailAuthorisation });

        #endregion Methods

    }

}
