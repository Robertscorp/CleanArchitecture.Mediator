using CleanArchitecture.Mediator.Internal;
using System;
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

        private IPipeHandle m_PipelineHandle;

        #endregion Fields

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
        {
            if (inputPort == null) throw new ArgumentNullException(nameof(inputPort));
            if (outputPort == null) throw new ArgumentNullException(nameof(outputPort));

            if (this.m_PipelineHandle == null)
            {
                var _PipelineHandleAccessor = (IPipelineHandleAccessor)serviceFactory(typeof(PipelineHandleAccessor<>).MakeGenericType(this.GetType()));

                this.m_PipelineHandle = _PipelineHandleAccessor.PipeHandle;
            }

            return PipelineInvoker.Instance(inputPort.GetType(), typeof(TOutputPort)).InvokePipelineAsync(inputPort, outputPort, this.m_PipelineHandle, serviceFactory, cancellationToken);
        }

        /// <summary>
        /// Invokes the pipeline.
        /// </summary>
        /// <typeparam name="TOutputPort">The type of output port.</typeparam>
        /// <param name="inputPort">The input to the pipeline.</param>
        /// <param name="outputPort">The output mechanism for the pipeline.</param>
        /// <param name="serviceFactory">The factory used to get service instances.</param>
        /// <param name="configureInvocationServiceCollection">The action to configure invocation-specific services.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <exception cref="ArgumentNullException"><paramref name="inputPort"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="outputPort"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="configureInvocationServiceCollection"/> is null.</exception>
        /// <remarks>
        /// The services defined in the <see cref="InvocationServiceCollection"/> will only be used when they are directly resolved from the 
        /// <paramref name="serviceFactory"/> within the pipeline. Any indirect resolutions will resolve using the original service.
        /// </remarks>
        public Task InvokeAsync<TOutputPort>(
            IInputPort<TOutputPort> inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            Action<InvocationServiceCollection> configureInvocationServiceCollection,
            CancellationToken cancellationToken)
        {
            if (inputPort == null) throw new ArgumentNullException(nameof(inputPort));
            if (outputPort == null) throw new ArgumentNullException(nameof(outputPort));
            if (configureInvocationServiceCollection == null) throw new ArgumentNullException(nameof(configureInvocationServiceCollection));

            if (this.m_PipelineHandle == null)
            {
                var _PipelineHandleAccessor = (IPipelineHandleAccessor)serviceFactory(typeof(PipelineHandleAccessor<>).MakeGenericType(this.GetType()));

                this.m_PipelineHandle = _PipelineHandleAccessor.PipeHandle;
            }

            var _InvocationServices = new InvocationServiceCollection(serviceFactory);
            configureInvocationServiceCollection(_InvocationServices);

            return PipelineInvoker.Instance(inputPort.GetType(), typeof(TOutputPort)).InvokePipelineAsync(inputPort, outputPort, this.m_PipelineHandle, _InvocationServices, cancellationToken);
        }

        #endregion Methods

    }

}
