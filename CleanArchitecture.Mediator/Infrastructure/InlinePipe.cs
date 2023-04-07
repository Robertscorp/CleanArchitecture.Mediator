using CleanArchitecture.Mediator.Pipeline;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Infrastructure
{

    internal class InlinePipe : IPipe
    {

        #region - - - - - - Fields - - - - - -

        private readonly Func<(object, object, ServiceFactory, NextPipeHandle, CancellationToken), Task> m_InlineBehaviourAsync;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public InlinePipe(Func<(object, object, ServiceFactory, NextPipeHandle, CancellationToken), Task> inlineBehaviourAsync)
            => this.m_InlineBehaviourAsync = inlineBehaviourAsync ?? throw new ArgumentNullException(nameof(inlineBehaviourAsync));

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        Task IPipe.InvokeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            PipeHandle nextPipeHandle,
            CancellationToken cancellationToken)
            => this.m_InlineBehaviourAsync(
                (inputPort, outputPort, serviceFactory, NextPipeHandle.GetNextPipeHandle(inputPort, outputPort, serviceFactory, nextPipeHandle, cancellationToken), cancellationToken));

        #endregion Methods

    }

    /// <summary>
    /// A handle to the next Pipe in the Pipeline.
    /// </summary>
    public abstract class NextPipeHandle
    {

        #region - - - - - - Methods - - - - - -

        internal static NextPipeHandle GetNextPipeHandle<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            PipeHandle nextPipeHandle,
            CancellationToken cancellationToken)
            => new NextPipeHandleInternal<TInputPort, TOutputPort>(inputPort, outputPort, serviceFactory, nextPipeHandle, cancellationToken);

        /// <summary>
        /// Invokes the next Pipe in the Pipeline.
        /// </summary>
        public abstract Task InvokePipeAsync();

        #endregion Methods

        #region - - - - - - Nested Classes - - - - - -

        private class NextPipeHandleInternal<TInputPort, TOutputPort> : NextPipeHandle
        {

            #region - - - - - - Fields - - - - - -

            private readonly TInputPort m_InputPort;
            private readonly TOutputPort m_OutputPort;
            private readonly ServiceFactory m_ServiceFactory;
            private readonly PipeHandle m_NextPipeHandle;
            private readonly CancellationToken m_CancellationToken;

            #endregion Fields

            #region - - - - - - Constructors - - - - - -

            public NextPipeHandleInternal(
                TInputPort inputPort,
                TOutputPort outputPort,
                ServiceFactory serviceFactory,
                PipeHandle nextPipeHandle,
                CancellationToken cancellationToken)
            {
                this.m_InputPort = inputPort;
                this.m_OutputPort = outputPort;
                this.m_ServiceFactory = serviceFactory;
                this.m_NextPipeHandle = nextPipeHandle;
                this.m_CancellationToken = cancellationToken;
            }

            #endregion Constructors

            #region - - - - - - Methods - - - - - -

            public override Task InvokePipeAsync()
                => this.m_NextPipeHandle.InvokePipeAsync(this.m_InputPort, this.m_OutputPort, this.m_ServiceFactory, this.m_CancellationToken);

            #endregion Methods

        }

        #endregion Nested Classes

    }

}
