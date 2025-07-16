using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Internal
{

    internal class NonGenericPipeHandle : IPipeHandle
    {

        #region - - - - - - Fields - - - - - -

        private readonly IPipeHandle m_NextPipeHandle;
        private readonly IPipe m_Pipe;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        internal NonGenericPipeHandle(IPipe pipe, IPipeHandle nextPipeHandle)
        {
            this.m_NextPipeHandle = nextPipeHandle;
            this.m_Pipe = pipe;
        }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        Task IPipeHandle.InvokePipeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            CancellationToken cancellationToken)
            => cancellationToken.IsCancellationRequested
                ? Task.FromCanceled(cancellationToken)
                : this.m_Pipe.InvokeAsync(inputPort, outputPort, serviceFactory, this.m_NextPipeHandle, cancellationToken);

        #endregion Methods

    }

}
