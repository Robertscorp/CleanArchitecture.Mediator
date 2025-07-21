using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Internal
{

    internal class TerminalPipeHandle : IPipeHandle
    {

        #region - - - - - - Methods - - - - - -

        Task IPipeHandle.InvokePipeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            CancellationToken cancellationToken)
            => Task.CompletedTask;

        #endregion Methods

    }

}
