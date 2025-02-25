using CleanArchitecture.Sample.Legacy.Authorisation;

namespace CleanArchitecture.Sample.UseCases.LegacyCreateProduct
{

    public class LegacyCreateProductAuthorisationEnforcer : IAuthorisationEnforcer<LegacyCreateProductInputPort, AuthorisationResult>
    {

        #region - - - - - - Methods - - - - - -

        public Task<AuthorisationResult> CheckAuthorisationAsync(LegacyCreateProductInputPort inputPort, CancellationToken cancellationToken)
            => Task.FromResult(new AuthorisationResult() { IsAuthorised = !inputPort.FailAuthorisation });

        #endregion Methods

    }

}
