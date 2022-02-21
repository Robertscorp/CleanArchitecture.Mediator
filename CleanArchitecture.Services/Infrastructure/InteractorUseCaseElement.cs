using CleanArchitecture.Services.Pipeline;

namespace CleanArchitecture.Services.Infrastructure
{

    /// <summary>
    /// Handles invocation of the Interactor service.
    /// </summary>
    public class InteractorUseCaseElement : IUseCaseElement
    {

        #region - - - - - - Fields - - - - - -

        private readonly UseCaseServiceResolver m_ServiceResolver;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Initialises a new instance of the <see cref="InteractorUseCaseElement"/> class.
        /// </summary>
        public InteractorUseCaseElement(UseCaseServiceResolver serviceResolver)
            => this.m_ServiceResolver = serviceResolver;

        #endregion Constructors

        #region - - - - - - IUseCaseElement Implementation - - - - - -

        Task IUseCaseElement.HandleAsync<TUseCaseInputPort, TUseCaseOutputPort>(
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
