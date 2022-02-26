using CleanArchitecture.Services.Pipeline;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Services.Infrastructure
{

    /// <summary>
    /// Handles invocation of the Interactor service.
    /// </summary>
    public class InteractorUseCaseElement : IUseCaseElement
    {

        #region - - - - - - Fields - - - - - -

        private readonly UseCaseServiceResolver m_ServiceResolver;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Initialises a new instance of the <see cref="InteractorUseCaseElement"/> class.
        /// </summary>
        /// <param name="serviceResolver">The delegate used to get services.</param>
        public InteractorUseCaseElement(UseCaseServiceResolver serviceResolver)
            => this.m_ServiceResolver = serviceResolver ?? throw new ArgumentNullException(nameof(serviceResolver));

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        Task IUseCaseElement.HandleAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            UseCaseElementHandleAsync nextUseCaseElementHandle,
            CancellationToken cancellationToken)
            => DelegateFactory.GetFunction<(TUseCaseInputPort, TUseCaseOutputPort, CancellationToken), Task>(
                typeof(InteractorHandleFactory<,>).MakeGenericType(typeof(TUseCaseInputPort), typeof(TUseCaseOutputPort)),
                this.m_ServiceResolver).Invoke((inputPort, outputPort, cancellationToken))
                    ?? Task.CompletedTask;

        #endregion Methods

        #region - - - - - - Nested Classes - - - - - -

        private class InteractorHandleFactory<TUseCaseInputPort, TUseCaseOutputPort>
            : IDelegateFactory<(TUseCaseInputPort, TUseCaseOutputPort, CancellationToken), Task>
            where TUseCaseInputPort : IUseCaseInputPort<TUseCaseOutputPort>
        {

            #region - - - - - - Methods - - - - - -

            public Func<(TUseCaseInputPort, TUseCaseOutputPort, CancellationToken), Task> GetFunction(
                UseCaseServiceResolver serviceResolver)
                => ipopc
                    => serviceResolver.GetService<IUseCaseInteractor<TUseCaseInputPort, TUseCaseOutputPort>>()?
                        .HandleAsync(ipopc.Item1, ipopc.Item2, ipopc.Item3);

            #endregion Methods

        }

        #endregion Nested Classes

    }

}
