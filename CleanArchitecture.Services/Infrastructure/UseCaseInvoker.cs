using CleanArchitecture.Services.Pipeline;

namespace CleanArchitecture.Services.Infrastructure
{

    public class UseCaseInvoker : IUseCaseInvoker
    {

        #region - - - - - - Fields - - - - - -

        private readonly UseCaseServiceResolver m_ServiceResolver;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public UseCaseInvoker(UseCaseServiceResolver serviceResolver)
            => this.m_ServiceResolver = serviceResolver;

        #endregion Constructors

        #region - - - - - - IUseCaseInvoker Implementation - - - - - -

        public Task InvokeUseCaseAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            CancellationToken cancellationToken)
            => this.m_ServiceResolver.GetService<IEnumerable<IUseCaseElement>>()!
                .Reverse()
                .Aggregate(
                    new UseCaseElementHandleAsync(() => Task.CompletedTask),
                    (nextElementHandleDelegate, useCaseElement) => new UseCaseElementHandleAsync(() => useCaseElement.HandleAsync(inputPort, outputPort, nextElementHandleDelegate, cancellationToken)))();

        #endregion IUseCaseInvoker Implementation

    }

}
