using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Internal
{

    internal class InlinePipe : IPipe
    {

        #region - - - - - - Fields - - - - - -

        private readonly Func<object, object, ServiceFactory, NextPipeHandleAsync, CancellationToken, Task> m_InlineBehaviourAsync;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public InlinePipe(Func<object, object, ServiceFactory, NextPipeHandleAsync, CancellationToken, Task> inlineBehaviourAsync)
            => this.m_InlineBehaviourAsync = inlineBehaviourAsync ?? throw new ArgumentNullException(nameof(inlineBehaviourAsync));

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        Task IPipe.InvokeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            IPipeHandle nextPipeHandle,
            CancellationToken cancellationToken)
            => this.m_InlineBehaviourAsync(
                inputPort,
                outputPort,
                serviceFactory,
                () => nextPipeHandle.InvokePipeAsync(inputPort, outputPort, serviceFactory, cancellationToken),
                cancellationToken);

        #endregion Methods

    }

}
