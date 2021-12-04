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

        public async Task InvokeUseCaseAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            CancellationToken cancellationToken)
        {
            var _UseCaseElements = (IEnumerable<IUseCaseElement>)this.m_ServiceProvider.GetService(typeof(IEnumerable<IUseCaseElement>))!;

            foreach (var _UseCaseElement in _UseCaseElements)
                if (await _UseCaseElement.TryOutputResultAsync(inputPort, outputPort, cancellationToken).ConfigureAwait(false))
                    return;
        }

        #endregion IUseCaseInvoker Implementation

    }

}
