using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Pipes
{

    /// <summary>
    /// Handles invocation of the Interactor service.
    /// </summary>
    public class InteractorInvocationPipe : IPipe
    {

        #region - - - - - - Methods - - - - - -

        Task IPipe.InvokeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            PipeHandle nextPipeHandle,
            CancellationToken cancellationToken)
            => serviceFactory
                .GetService<IInteractor<TInputPort, TOutputPort>>()?
                .HandleAsync(inputPort, outputPort, cancellationToken)
                    ?? Task.CompletedTask;

        #endregion Methods

    }

}
