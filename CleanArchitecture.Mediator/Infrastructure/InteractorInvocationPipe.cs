using CleanArchitecture.Mediator.Pipeline;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Infrastructure
{

    /// <summary>
    /// Handles invocation of the Interactor service.
    /// </summary>
    public class InteractorInvocationPipe : IUseCasePipe
    {

        #region - - - - - - Fields - - - - - -

        private readonly UseCaseServiceResolver m_ServiceResolver;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Initialises a new instance of the <see cref="InteractorInvocationPipe"/> class.
        /// </summary>
        /// <param name="serviceResolver">The delegate used to get services.</param>
        public InteractorInvocationPipe(UseCaseServiceResolver serviceResolver)
            => this.m_ServiceResolver = serviceResolver ?? throw new ArgumentNullException(nameof(serviceResolver));

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        Task IUseCasePipe.HandleAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            UseCasePipeHandleAsync nextUseCasePipeHandle,
            CancellationToken cancellationToken)
            => DelegateFactory
                .GetFunction<(UseCaseServiceResolver, TUseCaseInputPort, TUseCaseOutputPort, CancellationToken), Task>(
                    typeof(InteractorHandleFactory<,>).MakeGenericType(typeof(TUseCaseInputPort), typeof(TUseCaseOutputPort)))?
                .Invoke((this.m_ServiceResolver, inputPort, outputPort, cancellationToken))
                    ?? Task.CompletedTask;

        #endregion Methods

        #region - - - - - - Nested Classes - - - - - -

        private class InteractorHandleFactory<TUseCaseInputPort, TUseCaseOutputPort>
            : IDelegateFactory<(UseCaseServiceResolver, TUseCaseInputPort, TUseCaseOutputPort, CancellationToken), Task>
            where TUseCaseInputPort : IUseCaseInputPort<TUseCaseOutputPort>
        {

            #region - - - - - - Methods - - - - - -

            public Func<(UseCaseServiceResolver, TUseCaseInputPort, TUseCaseOutputPort, CancellationToken), Task> GetFunction()
                => sripopc
                    => sripopc.Item1
                        .GetService<IUseCaseInteractor<TUseCaseInputPort, TUseCaseOutputPort>>()?
                        .HandleAsync(sripopc.Item2, sripopc.Item3, sripopc.Item4);

            #endregion Methods

        }

        #endregion Nested Classes

    }

}
