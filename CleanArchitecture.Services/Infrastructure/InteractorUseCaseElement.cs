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

        public Task HandleAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            UseCaseElementHandleAsync nextUseCaseElementHandle,
            CancellationToken cancellationToken)
        {
            var _Interactor = (IUseCaseInteractor<TUseCaseInputPort, TUseCaseOutputPort>?)this.m_ServiceProvider.GetService(typeof(IUseCaseInteractor<TUseCaseInputPort, TUseCaseOutputPort>));
            return _Interactor?.HandleAsync(inputPort, outputPort, cancellationToken) ?? Task.CompletedTask;
        }

        #endregion IUseCaseElement Implementation

    }

}
