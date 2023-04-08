using CleanArchitecture.Mediator.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Pipes
{

    /// <summary>
    /// Handles invocation of the Interactor service.
    /// </summary>
    public class InteractorInvocationPipe : IPipe
    {

        #region - - - - - - Methods - - - - - -

        Task IPipe.InvokeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            PipeHandle nextPipeHandle,
            CancellationToken cancellationToken)
            => DelegateFactory
                .GetFunction<(ServiceFactory, TInputPort, TOutputPort, CancellationToken), Task>(
                    typeof(InteractorHandleFactory<,>).MakeGenericType(typeof(TInputPort), typeof(TOutputPort)))?
                .Invoke((serviceFactory, inputPort, outputPort, cancellationToken))
                    ?? Task.CompletedTask;

        #endregion Methods

        #region - - - - - - Nested Classes - - - - - -

        private class InteractorHandleFactory<TInputPort, TOutputPort>
            : IDelegateFactory<(ServiceFactory, TInputPort, TOutputPort, CancellationToken), Task>
            where TInputPort : IUseCaseInputPort<TOutputPort>
        {

            #region - - - - - - Methods - - - - - -

            public Func<(ServiceFactory, TInputPort, TOutputPort, CancellationToken), Task> GetFunction()
                => tuple
                    => tuple.Item1
                        .GetService<IUseCaseInteractor<TInputPort, TOutputPort>>()?
                        .HandleAsync(tuple.Item2, tuple.Item3, tuple.Item4);

            #endregion Methods

        }

        #endregion Nested Classes

    }

}
