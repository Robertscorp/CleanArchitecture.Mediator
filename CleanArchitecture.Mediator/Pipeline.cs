using CleanArchitecture.Mediator.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// A Pipeline that can be configured and invoked.
    /// </summary>
    public class Pipeline : IPipeline
    {

        #region - - - - - - Fields - - - - - -

        private readonly ServiceFactory m_ServiceFactory;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Initialises a new instance of the <see cref="Pipeline"/> class.
        /// </summary>
        /// <param name="serviceFactory">The factory used to get the service object of the specified type.</param>
        public Pipeline(ServiceFactory serviceFactory)
            => this.m_ServiceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        private Invoker GetInvoker<TOutputPort>(IInputPort<TOutputPort> inputPort, TOutputPort outputPort)
            => (Invoker)Activator.CreateInstance(
                typeof(Invoker<,,>).MakeGenericType(this.GetType(), inputPort.GetType(), typeof(TOutputPort)),
                inputPort,
                outputPort);

        /// <summary>
        /// Invokes the Pipeline.
        /// </summary>
        /// <typeparam name="TOutputPort">The type of Output Port.</typeparam>
        /// <param name="inputPort">The input to the pipeline.</param>
        /// <param name="outputPort">The output mechanism for the pipeline.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        public Task InvokeAsync<TOutputPort>(
            IInputPort<TOutputPort> inputPort,
            TOutputPort outputPort,
            CancellationToken cancellationToken)
            => this.GetInvoker(inputPort, outputPort)
                .InvokePipelineAsync(this.m_ServiceFactory, cancellationToken);

        #endregion Methods

        #region - - - - - - Nested Classes - - - - - -

        private abstract class Invoker
        {

            #region - - - - - - Methods - - - - - -

            public abstract Task InvokePipelineAsync(ServiceFactory serviceFactory, CancellationToken cancellationToken);

            #endregion Methods

        }

        private class Invoker<TPipeline, TInputPort, TOutputPort> : Invoker
            where TPipeline : IPipeline
            where TInputPort : IInputPort<TOutputPort>
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

            public override Task InvokePipelineAsync(ServiceFactory serviceFactory, CancellationToken cancellationToken)
                => serviceFactory
                    .GetService<IPipelineHandleFactory>()
                    .GetPipelineHandle<TPipeline>()
                    .InvokePipeAsync(this.m_InputPort, this.m_OutputPort, serviceFactory, cancellationToken);

            #endregion Methods

        }

        #endregion Nested Classes

    }

}
