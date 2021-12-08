using CleanArchitecture.Services.Pipeline.Authorisation;

namespace CleanArchitecture.Services.Pipeline.Infrastructure
{

    public class AuthorisationUseCaseElement<TAuthorisationResult> : IUseCaseElement where TAuthorisationResult : IAuthorisationResult
    {

        #region - - - - - - Fields - - - - - -

        private readonly UseCaseServiceResolver m_ServiceResolver;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public AuthorisationUseCaseElement(UseCaseServiceResolver serviceResolver)
            => this.m_ServiceResolver = serviceResolver;

        #endregion Constructors

        #region - - - - - - IUseCaseElement Implementation - - - - - -

        public async Task HandleAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            UseCaseElementHandleAsync nextUseCaseElementHandle,
            CancellationToken cancellationToken)
        {
            if (outputPort is IAuthorisationOutputPort<TAuthorisationResult> _OutputPort)
            {
                var _AuthorisationResultAsync = this.GetAuthorisationResultAsync(inputPort, cancellationToken);
                if (_AuthorisationResultAsync != null && !(await _AuthorisationResultAsync).IsAuthorised)
                {
                    await _OutputPort.PresentUnauthorisedAsync(await _AuthorisationResultAsync, cancellationToken).ConfigureAwait(false);
                    return;
                }
            }

            await nextUseCaseElementHandle().ConfigureAwait(false);
        }

        #endregion IUseCaseElement Implementation

        #region - - - - - - Methods - - - - - -

        private Task<TAuthorisationResult>? GetAuthorisationResultAsync<TUseCaseInputPort>(TUseCaseInputPort inputPort, CancellationToken cancellationToken)
            => this.m_ServiceResolver
                .GetService<IUseCaseAuthorisationEnforcer<TUseCaseInputPort, TAuthorisationResult>>()?
                .CheckAuthorisationAsync(inputPort, cancellationToken);

        #endregion Methods

    }

}
