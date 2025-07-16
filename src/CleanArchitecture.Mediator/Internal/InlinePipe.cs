using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Internal
{

    internal class InlinePipe : IPipeHandle
    {

        #region - - - - - - Fields - - - - - -

        private readonly Func<object, object, ServiceFactory, NextPipeHandleAsync, CancellationToken, Task> m_InlineBehaviourAsync;
        private readonly IPipeHandle m_NextPipeHandle;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public InlinePipe(Func<object, object, ServiceFactory, NextPipeHandleAsync, CancellationToken, Task> inlineBehaviourAsync, IPipeHandle nextPipeHandle)
        {
            this.m_InlineBehaviourAsync = inlineBehaviourAsync;
            this.m_NextPipeHandle = nextPipeHandle;
        }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        Task IPipeHandle.InvokePipeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            CancellationToken cancellationToken)
            => this.m_InlineBehaviourAsync(
                inputPort,
                outputPort,
                serviceFactory,
                () => this.m_NextPipeHandle.InvokePipeAsync(inputPort, outputPort, serviceFactory, cancellationToken),
                cancellationToken);

        #endregion Methods

    }

}
