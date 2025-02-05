using CleanArchitecture.Mediator.Internal;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// A pipeline that can be configured and invoked.
    /// </summary>
    public class Pipeline
    {

        #region - - - - - - Fields - - - - - -

        private readonly PipeHandle m_PipelineHandle;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Initialises a new instance of the <see cref="Pipeline"/> class.
        /// </summary>
        /// <param name="serviceFactory">The factory used to get service instances.</param>
        public Pipeline(ServiceFactory serviceFactory)
        {
            if (serviceFactory == null)
                throw new ArgumentNullException(nameof(serviceFactory));

            var _PipelineHandleAccessor = (IPipelineHandleAccessor)serviceFactory(typeof(PipelineHandleAccessor<>).MakeGenericType(this.GetType()));

            this.m_PipelineHandle = _PipelineHandleAccessor.PipeHandle;
        }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

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
                    : PipelineInvoker.Instance(inputPort.GetType(), typeof(TOutputPort)).InvokePipelineAsync(inputPort, outputPort, this.m_PipelineHandle, serviceFactory, cancellationToken);

        #endregion Methods

        #region - - - - - - Nested Classes - - - - - -

        internal abstract class PipelineInvoker
        {

            #region - - - - - - Fields - - - - - -

            private static readonly ConcurrentDictionary<(Type, Type), PipelineInvoker> s_Cache = new ConcurrentDictionary<(Type, Type), PipelineInvoker>();

            #endregion Fields

            #region - - - - - - Methods - - - - - -

            public static PipelineInvoker Instance(Type inputPortType, Type outputPortType)
            {
                var _Key = (inputPortType, outputPortType);
                if (!s_Cache.TryGetValue(_Key, out var _Invoker))
                    _Invoker = s_Cache.GetOrAdd(_Key, (PipelineInvoker)Activator.CreateInstance(typeof(PipelineInvoker<,>).MakeGenericType(inputPortType, outputPortType)));

                return _Invoker;
            }

            public abstract Task InvokePipelineAsync(
                object inputPort,
                object outputPort,
                PipeHandle pipelineHandle,
                ServiceFactory serviceFactory,
                CancellationToken cancellationToken);

            #endregion Methods

        }

        internal class PipelineInvoker<TInputPort, TOutputPort> : PipelineInvoker where TInputPort : IInputPort<TOutputPort>
        {

            #region - - - - - - Methods - - - - - -

            public override Task InvokePipelineAsync(object inputPort, object outputPort, PipeHandle pipelineHandle, ServiceFactory serviceFactory, CancellationToken cancellationToken)
                => pipelineHandle.InvokePipeAsync((TInputPort)inputPort, (TOutputPort)outputPort, serviceFactory, cancellationToken);

            #endregion Methods

        }

        #endregion Nested Classes

    }

}
