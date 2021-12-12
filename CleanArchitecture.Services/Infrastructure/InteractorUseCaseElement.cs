using CleanArchitecture.Services.Pipeline;

namespace CleanArchitecture.Services.Infrastructure
{

    public class InteractorUseCaseElement : IUseCaseElement
    {

        #region - - - - - - Fields - - - - - -

        private readonly UseCaseServiceResolver m_ServiceResolver;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public InteractorUseCaseElement(UseCaseServiceResolver serviceResolver)
            => this.m_ServiceResolver = serviceResolver;

        #endregion Constructors

        #region - - - - - - IUseCaseElement Implementation - - - - - -

        public Task HandleAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            UseCaseElementHandleAsync nextUseCaseElementHandle,
            CancellationToken cancellationToken)
        {
            var _Interactor = this.m_ServiceResolver.GetService<IUseCaseInteractor<TUseCaseInputPort, TUseCaseOutputPort>>();
            return _Interactor != null
                ? _Interactor.HandleAsync(inputPort, outputPort, cancellationToken)
                : this.GetInteractorInvoker(inputPort, outputPort)?.InvokeAsync(cancellationToken)
                    ?? Task.CompletedTask;
        }

        #endregion IUseCaseElement Implementation

        #region - - - - - - Methods - - - - - -

        private InteractorInvoker? GetInteractorInvoker<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort)
        {
            var _OutputType = GetOutputPortInterfaceType(typeof(TUseCaseOutputPort));
            return _OutputType == null
                ? null
                : (InteractorInvoker)Activator.CreateInstance(
                    typeof(InteractorInvoker<,>).MakeGenericType(typeof(TUseCaseInputPort), _OutputType),
                    inputPort,
                    outputPort,
                    this.m_ServiceResolver)!;
        }

        private static Type? GetOutputPortInterfaceType(Type outputPortImplementationType)
            => outputPortImplementationType.GetInterfaces().FirstOrDefault();

        #endregion Methods

        #region - - - - - - Nested Classes - - - - - -

        private abstract class InteractorInvoker
        {

            #region - - - - - - Methods - - - - - -

            public abstract Task InvokeAsync(CancellationToken cancellationToken);

            #endregion Methods

        }

        private class InteractorInvoker<TUseCaseInputPort, TUseCaseOutputPort> : InteractorInvoker
        {

            #region - - - - - - Fields - - - - - -

            private readonly TUseCaseInputPort m_InputPort;
            private readonly TUseCaseOutputPort m_OutputPort;
            private readonly UseCaseServiceResolver m_ServiceResolver;

            #endregion Fields

            #region - - - - - - Constructors - - - - - -

            public InteractorInvoker(
                TUseCaseInputPort inputPort,
                TUseCaseOutputPort outputPort,
                UseCaseServiceResolver serviceResolver)
            {
                this.m_InputPort = inputPort;
                this.m_OutputPort = outputPort;
                this.m_ServiceResolver = serviceResolver;
            }

            #endregion Constructors

            #region - - - - - - Methods - - - - - -

            public override Task InvokeAsync(CancellationToken cancellationToken)
                => this.m_ServiceResolver
                    .GetService<IUseCaseInteractor<TUseCaseInputPort, TUseCaseOutputPort>>()?
                    .HandleAsync(this.m_InputPort, this.m_OutputPort, cancellationToken)
                        ?? Task.CompletedTask;

            #endregion Methods

        }

        #endregion Nested Classes

    }

}
