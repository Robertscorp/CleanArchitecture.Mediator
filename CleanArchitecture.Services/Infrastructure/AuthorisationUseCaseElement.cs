using CleanArchitecture.Services.Pipeline;

namespace CleanArchitecture.Services.Infrastructure
{

    /// <summary>
    /// Handles invocation of the Authorisation Enforcer service and presentation of authorisation failures.
    /// </summary>
    /// <typeparam name="TAuthorisationResult">The type of Authorisation Result.</typeparam>
    public class AuthorisationUseCaseElement<TAuthorisationResult> : IUseCaseElement where TAuthorisationResult : IAuthorisationResult
    {

        #region - - - - - - Fields - - - - - -

        private readonly UseCaseServiceResolver m_ServiceResolver;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Initialises a new instance of the <see cref="AuthorisationUseCaseElement{TAuthorisationResult}"/> class.
        /// </summary>
        public AuthorisationUseCaseElement(UseCaseServiceResolver serviceResolver)
            => this.m_ServiceResolver = serviceResolver;

        #endregion Constructors

        #region - - - - - - IUseCaseElement Implementation - - - - - -

        async Task IUseCaseElement.HandleAsync<TUseCaseInputPort, TUseCaseOutputPort>(
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
            => inputPort is IUseCaseInputPort<IAuthorisationOutputPort<TAuthorisationResult>>
                ? this.GetEnforcerInvoker(inputPort)?.GetAuthorisationResultAsync(cancellationToken)
                : null;

        private EnforcerInvoker? GetEnforcerInvoker<TUseCaseInputPort>(TUseCaseInputPort inputPort)
            => (EnforcerInvoker?)Activator.CreateInstance(
                typeof(EnforcerInvoker<>).MakeGenericType(typeof(TAuthorisationResult), typeof(TUseCaseInputPort)),
                inputPort,
                this.m_ServiceResolver);

        #endregion Methods

        #region - - - - - - Nested Classes - - - - - -

        private abstract class EnforcerInvoker
        {

            #region - - - - - - Methods - - - - - -

            public abstract Task<TAuthorisationResult>? GetAuthorisationResultAsync(CancellationToken cancellationToken);

            #endregion Methods

        }

        private class EnforcerInvoker<TUseCaseInputPort> : EnforcerInvoker
            where TUseCaseInputPort : IUseCaseInputPort<IAuthorisationOutputPort<TAuthorisationResult>>
        {

            #region - - - - - - Fields - - - - - -

            private readonly TUseCaseInputPort m_InputPort;
            private readonly UseCaseServiceResolver m_ServiceResolver;

            #endregion Fields

            #region - - - - - - Constructors - - - - - -

            public EnforcerInvoker(TUseCaseInputPort inputPort, UseCaseServiceResolver serviceResolver)
            {
                this.m_InputPort = inputPort;
                this.m_ServiceResolver = serviceResolver;
            }

            #endregion Constructors

            #region - - - - - - Methods - - - - - -

            public override Task<TAuthorisationResult>? GetAuthorisationResultAsync(CancellationToken cancellationToken)
                => this.m_ServiceResolver
                    .GetService<IUseCaseAuthorisationEnforcer<TUseCaseInputPort, TAuthorisationResult>>()?
                    .CheckAuthorisationAsync(this.m_InputPort, cancellationToken);

            #endregion Methods

        }

        #endregion Nested Classes

    }

}
