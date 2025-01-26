using CleanArchitecture.Mediator.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// A pipeline that can be configured and invoked.
    /// </summary>
    public class Pipeline : IPipeline
    {

        #region - - - - - - Fields - - - - - -

        private readonly PipeHandle m_PipelineHandle;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Initialises a new instance of the <see cref="Pipeline"/> class.
        /// </summary>
        /// <param name="pipelineHandleFactory">The factory used to get a <see cref="PipeHandle"/> for the <see cref="Pipeline"/>.</param>
        public Pipeline(IPipelineHandleFactory pipelineHandleFactory)
            => this.m_PipelineHandle = (pipelineHandleFactory ?? throw new ArgumentNullException(nameof(pipelineHandleFactory))).GetPipelineHandle(this);

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        private Invoker GetInvoker<TOutputPort>(IInputPort<TOutputPort> inputPort, TOutputPort outputPort)
            => (Invoker)Activator.CreateInstance(
                typeof(Invoker<,>).MakeGenericType(inputPort.GetType(), typeof(TOutputPort)),
                inputPort,
                outputPort);

        /// <summary>
        /// Invokes the pipeline.
        /// </summary>
        /// <typeparam name="TOutputPort">The type of output port.</typeparam>
        /// <param name="inputPort">The input to the pipeline.</param>
        /// <param name="outputPort">The output mechanism for the pipeline.</param>
        /// <param name="serviceFactory">The factory used to get service instances.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <exception cref="ArgumentNullException"><paramref name="inputPort"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="outputPort"/> is null.</exception>
        public Task InvokeAsync<TOutputPort>(
            IInputPort<TOutputPort> inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            CancellationToken cancellationToken)
            => inputPort == null
                ? throw new ArgumentNullException(nameof(inputPort))
                : outputPort == null
                    ? throw new ArgumentNullException(nameof(outputPort))
                    : this.GetInvoker(inputPort, outputPort)
                        .InvokePipelineAsync(this.m_PipelineHandle, serviceFactory, cancellationToken);

        #endregion Methods

        #region - - - - - - Nested Classes - - - - - -

        private abstract class Invoker
        {

            #region - - - - - - Methods - - - - - -

            public abstract Task InvokePipelineAsync(PipeHandle pipelineHandle, ServiceFactory serviceFactory, CancellationToken cancellationToken);

            #endregion Methods

        }

        private class Invoker<TInputPort, TOutputPort> : Invoker where TInputPort : IInputPort<TOutputPort>
        {

            #region - - - - - - Fields - - - - - -

            private readonly TInputPort m_InputPort;
            private readonly TOutputPort m_OutputPort;

            #endregion Fields

            #region - - - - - - Constructors - - - - - -

            public Invoker(TInputPort inputPort, TOutputPort outputPort)
            {
                this.m_InputPort = inputPort;
                this.m_OutputPort = outputPort;
            }

            #endregion Constructors

            #region - - - - - - Methods - - - - - -

            public override Task InvokePipelineAsync(PipeHandle pipelineHandle, ServiceFactory serviceFactory, CancellationToken cancellationToken)
                => pipelineHandle.InvokePipeAsync(this.m_InputPort, this.m_OutputPort, serviceFactory, cancellationToken);

            #endregion Methods

        }

        #endregion Nested Classes

    }

}
