using CleanArchitecture.Services.Pipeline;

namespace CleanArchitecture.Services.Infrastructure
{

    public class InteractorUseCaseElement : IUseCaseElement
    {

        #region - - - - - - Fields - - - - - -

        private readonly UseCaseServiceResolver m_ServiceResolver;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public InteractorUseCaseElement(UseCaseServiceResolver serviceResolver)
            => this.m_ServiceResolver = serviceResolver;

        #endregion Constructors

        #region - - - - - - IUseCaseElement Implementation - - - - - -

        public Task HandleAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            UseCaseElementHandleAsync nextUseCaseElementHandle,
            CancellationToken cancellationToken)
            => this.m_ServiceResolver.GetService<IUseCaseInteractor<TUseCaseInputPort, TUseCaseOutputPort>>()?
                .HandleAsync(inputPort, outputPort, cancellationToken)
                    ?? Task.CompletedTask;

        #endregion IUseCaseElement Implementation

    }

}
