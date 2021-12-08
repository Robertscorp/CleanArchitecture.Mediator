using CleanArchitecture.Services.Pipeline;

namespace CleanArchitecture.Services.Infrastructure
{

    public class UseCaseInvoker : IUseCaseInvoker
    {

        #region - - - - - - Fields - - - - - -

        private readonly IServiceProvider m_ServiceProvider;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public UseCaseInvoker(IServiceProvider serviceProvider)
            => this.m_ServiceProvider = serviceProvider;

        #endregion Constructors

        #region - - - - - - IUseCaseInvoker Implementation - - - - - -

        public Task InvokeUseCaseAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            CancellationToken cancellationToken)
        {
            var _UseCaseElements = (IEnumerable<IUseCaseElement>)this.m_ServiceProvider.GetService(typeof(IEnumerable<IUseCaseElement>))!;
            return _UseCaseElements
                .Reverse()
                .Aggregate(
                    new UseCaseElementHandleAsync(() => Task.CompletedTask),
                    (nextElementHandleDelegate, useCaseElement) => new UseCaseElementHandleAsync(() => useCaseElement.HandleAsync(inputPort, outputPort, nextElementHandleDelegate, cancellationToken)))();
        }

        #endregion IUseCaseInvoker Implementation

    }

}
