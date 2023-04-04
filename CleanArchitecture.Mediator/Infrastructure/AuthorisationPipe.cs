using CleanArchitecture.Mediator.Pipeline;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Infrastructure
{

    /// <summary>
    /// Handles invocation of the Authorisation Enforcer service and presentation of authorisation failures.
    /// </summary>
    /// <typeparam name="TAuthorisationResult">The type of Authorisation Result.</typeparam>
    public class AuthorisationPipe<TAuthorisationResult> : IPipe where TAuthorisationResult : IAuthorisationResult
    {

        #region - - - - - - Methods - - - - - -

        private Task<TAuthorisationResult> GetAuthorisationResultAsync<TUseCaseInputPort>(TUseCaseInputPort inputPort, ServiceFactory serviceFactory, CancellationToken cancellationToken)
            => inputPort is IUseCaseInputPort<IAuthorisationOutputPort<TAuthorisationResult>>
                ? DelegateFactory
                    .GetFunction<(ServiceFactory, TUseCaseInputPort, CancellationToken), Task<TAuthorisationResult>>(
                        typeof(EnforcerCheckFactory<>).MakeGenericType(typeof(TAuthorisationResult), typeof(TUseCaseInputPort)))?
                    .Invoke((serviceFactory, inputPort, cancellationToken))
                : null;

        async Task IPipe.InvokeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            PipeHandle nextPipeHandle,
            CancellationToken cancellationToken)
        {
            if (outputPort is IAuthorisationOutputPort<TAuthorisationResult> _OutputPort)
            {
                var _AuthorisationResultAsync = this.GetAuthorisationResultAsync(inputPort, serviceFactory, cancellationToken);
                if (_AuthorisationResultAsync != null)
                {
                    var _AuthorisationResult = await _AuthorisationResultAsync.ConfigureAwait(false);
                    if (!_AuthorisationResult.IsAuthorised)
                    {
                        await _OutputPort.PresentUnauthorisedAsync(_AuthorisationResult, cancellationToken).ConfigureAwait(false);
                        return;
                    }
                }
            }

            await nextPipeHandle.InvokePipeAsync(inputPort, outputPort, serviceFactory, default).ConfigureAwait(false);
        }

        #endregion Methods

        #region - - - - - - Nested Classes - - - - - -

        private class EnforcerCheckFactory<TInputPort>
            : IDelegateFactory<(ServiceFactory, TInputPort, CancellationToken), Task<TAuthorisationResult>>
            where TInputPort : IUseCaseInputPort<IAuthorisationOutputPort<TAuthorisationResult>>
        {

            #region - - - - - - Methods - - - - - -

            public Func<(ServiceFactory, TInputPort, CancellationToken), Task<TAuthorisationResult>> GetFunction()
                => tuple
                    => tuple.Item1
                        .GetService<IUseCaseAuthorisationEnforcer<TInputPort, TAuthorisationResult>>()?
                        .CheckAuthorisationAsync(tuple.Item2, tuple.Item3);

            #endregion Methods

        }

        #endregion Nested Classes

    }

}
