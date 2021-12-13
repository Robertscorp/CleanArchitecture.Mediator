using CleanArchitecture.Services.Pipeline;

namespace CleanArchitecture.Services.Infrastructure
{

    public class UseCaseInvoker : IUseCaseInvoker
    {

        #region - - - - - - Fields - - - - - -

        private readonly UseCaseServiceResolver m_ServiceResolver;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public UseCaseInvoker(UseCaseServiceResolver serviceResolver)
            => this.m_ServiceResolver = serviceResolver;

        #endregion Constructors

        #region - - - - - - IUseCaseInvoker Implementation - - - - - -

        public Task InvokeUseCaseAsync<TUseCaseOutputPort>(
            IUseCaseInputPort<TUseCaseOutputPort> inputPort,
            TUseCaseOutputPort outputPort,
            CancellationToken cancellationToken)
            => this.GetUseCaseInvoker(inputPort, outputPort)
                .InvokeUseCaseAsync(cancellationToken);

        #endregion IUseCaseInvoker Implementation

        #region - - - - - - Methods - - - - - -

        private Invoker GetUseCaseInvoker<TUseCaseOutputPort>(IUseCaseInputPort<TUseCaseOutputPort> inputPort, TUseCaseOutputPort outputPort)
            => (Invoker)Activator.CreateInstance(
                typeof(Invoker<,>).MakeGenericType(inputPort.GetType(), typeof(TUseCaseOutputPort)),
                inputPort,
                outputPort,
                this.m_ServiceResolver)!;

        #endregion Methods

        #region - - - - - - Nested Classes - - - - - -

        #endregion Nested Classes

        private abstract class Invoker
        {

            #region - - - - - - Methods - - - - - -

            public abstract Task InvokeUseCaseAsync(CancellationToken cancellationToken);

            #endregion Methods

        }

        private class Invoker<TUseCaseInputPort, TUseCaseOutputPort> : Invoker
        {

            #region - - - - - - Fields - - - - - -

            private readonly TUseCaseInputPort m_InputPort;
            private readonly TUseCaseOutputPort m_OutputPort;
            private readonly UseCaseServiceResolver m_ServiceResolver;

            #endregion Fields

            #region - - - - - - Constructors - - - - - -

            public Invoker(TUseCaseInputPort inputPort, TUseCaseOutputPort outputPort, UseCaseServiceResolver serviceResolver)
            {
                this.m_InputPort = inputPort;
                this.m_OutputPort = outputPort;
                this.m_ServiceResolver = serviceResolver;
            }

            #endregion Constructors

            #region - - - - - - Methods - - - - - -

            public override Task InvokeUseCaseAsync(CancellationToken cancellationToken)
                => this.m_ServiceResolver.GetService<IEnumerable<IUseCaseElement>>()!
                    .Reverse()
                    .Aggregate(
                        new UseCaseElementHandleAsync(() => Task.CompletedTask),
                        (nextElementHandleDelegate, useCaseElement) =>
                            new UseCaseElementHandleAsync(() => useCaseElement.HandleAsync(this.m_InputPort, this.m_OutputPort, nextElementHandleDelegate, cancellationToken)))();

            #endregion Methods

        }

    }

}
