using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Internal
{

    internal class InteractorInvocationPipe : IPipe
    {

        #region - - - - - - Methods - - - - - -

        Task IPipe.InvokeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            NextPipeHandleAsync nextPipeHandle,
            CancellationToken cancellationToken)
            => serviceFactory
                .GetService<IInteractor<TInputPort, TOutputPort>>()?
                .HandleAsync(inputPort, outputPort, serviceFactory, cancellationToken)
                    ?? Task.CompletedTask;

        #endregion Methods

    }

}
