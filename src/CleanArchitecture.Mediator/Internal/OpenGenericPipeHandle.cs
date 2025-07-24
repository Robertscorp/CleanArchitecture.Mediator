using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Internal
{

    internal class OpenGenericPipeHandle : IPipeHandle
    {

        #region - - - - - - Fields - - - - - -

        private readonly IPipeHandle m_NextPipeHandle;
        private readonly IClosedGenericPipeProvider m_PipeProvider;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        internal OpenGenericPipeHandle(IClosedGenericPipeProvider pipeProvider, IPipeHandle nextPipeHandle)
        {
            this.m_NextPipeHandle = nextPipeHandle;
            this.m_PipeProvider = pipeProvider;
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
                : this.m_PipeProvider.GetPipe<TInputPort, TOutputPort>(serviceFactory)?
                    .InvokeAsync(
                        inputPort,
                        outputPort,
                        serviceFactory,
                        () => this.m_NextPipeHandle.InvokePipeAsync(inputPort, outputPort, serviceFactory, cancellationToken),
                        cancellationToken)
                            ?? this.m_NextPipeHandle.InvokePipeAsync(inputPort, outputPort, serviceFactory, cancellationToken);

        #endregion Methods

    }

}
