using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Internal
{

    internal class OpenGenericPipeHandle : IPipeHandle
    {

        #region - - - - - - Fields - - - - - -

        private readonly IPipeHandle m_NextPipeHandle;
        private readonly Func<Type, Type, ServiceFactory, object> m_PipeProvider;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        internal OpenGenericPipeHandle(Type openGenericPipeType, Func<Type, Type, Type[]> getClosedGenericParameters, IPipeHandle nextPipeHandle)
        {
            this.m_NextPipeHandle = nextPipeHandle;

            var _PipeCache = new ConcurrentDictionary<Type, object>();

            this.m_PipeProvider = (inputPort, outputPort, serviceFactory)
                => _PipeCache.GetOrAdd(inputPort, _ =>
                {
                    try // Try-Catch is the easiest way of determining if all generic constraints are met.
                    {
                        return serviceFactory(openGenericPipeType.MakeGenericType(getClosedGenericParameters(inputPort, outputPort)));
                    }
                    catch
                    {
                        return null;
                    }
                });
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
                : ((IPipe<TInputPort, TOutputPort>)this.m_PipeProvider(typeof(TInputPort), typeof(TOutputPort), serviceFactory))?
                    .InvokeAsync(inputPort, outputPort, serviceFactory, this.m_NextPipeHandle, cancellationToken)
                        ?? this.m_NextPipeHandle.InvokePipeAsync(inputPort, outputPort, serviceFactory, cancellationToken);

        #endregion Methods

    }

}
