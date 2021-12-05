using CleanArchitecture.Services.Pipeline;

namespace CleanArchitecture.Services.Infrastructure
{

    public class InteractorUseCaseElement : IUseCaseElement
    {

        #region - - - - - - Fields - - - - - -

        private readonly IServiceProvider m_ServiceProvider;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public InteractorUseCaseElement(IServiceProvider serviceProvider)
            => this.m_ServiceProvider = serviceProvider;

        #endregion Constructors

        #region - - - - - - IUseCaseElement Implementation - - - - - -

        public async Task<bool> TryOutputResultAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            CancellationToken cancellationToken)
        {
            var _Interactor = (IUseCaseInteractor<TUseCaseInputPort, TUseCaseOutputPort>?)this.m_ServiceProvider.GetService(typeof(IUseCaseInteractor<TUseCaseInputPort, TUseCaseOutputPort>));
            if (_Interactor == null)
                return false;

            await _Interactor.HandleAsync(inputPort, outputPort, cancellationToken).ConfigureAwait(false);

            return true;
        }

        #endregion IUseCaseElement Implementation

    }

}
