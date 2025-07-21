using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Internal
{

    /// <summary>
    /// This class exists as a mechanism to change the reference type of TInputPort.
    /// </summary>
    /// <remarks>
    /// Since the pipeline is invoked with the input port referenced as IInputPort&lt;TOutputPort&gt;, the pipes would be invoked
    /// with TInputPort being an IInputPort. When the pipes try to resolve pipe services, they would then be resolved with TInputPort as 
    /// an IInputPort. This would force all pipe services to be implemented with their TInputPort as IInputPort, otherwise the service
    /// factory wouldn't be able to resolve them. This would then force the consumers to cast the input port parameter to the implementation
    /// type in every pipe service, which is terrible UX. Far better UX is to have the pipes be invoked with TInputPort as the implementation
    /// type of the input port instance, and TOutputPort as the interface type of the output port instance.
    /// </remarks>
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
            IPipeHandle pipelineHandle,
            ServiceFactory serviceFactory,
            CancellationToken cancellationToken);

        #endregion Methods

    }

    internal class PipelineInvoker<TInputPort, TOutputPort> : PipelineInvoker where TInputPort : IInputPort<TOutputPort>
    {

        #region - - - - - - Methods - - - - - -

        public override Task InvokePipelineAsync(object inputPort, object outputPort, IPipeHandle pipelineHandle, ServiceFactory serviceFactory, CancellationToken cancellationToken)
            => pipelineHandle.InvokePipeAsync((TInputPort)inputPort, (TOutputPort)outputPort, serviceFactory, cancellationToken);

        #endregion Methods

    }

}
