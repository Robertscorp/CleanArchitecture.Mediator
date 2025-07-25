﻿namespace CleanArchitecture.Mediator.Sample.Legacy.Authorisation;

/// <summary>
/// Handles invocation of the authorisation enforcer service and presentation of authorisation failures.
/// </summary>
/// <typeparam name="TAuthorisationResult">The type of authorisation result.</typeparam>
public class AuthorisationPipe<TAuthorisationResult> : IPipe where TAuthorisationResult : IAuthorisationResult
{

    #region - - - - - - Methods - - - - - -

    private static Task<TAuthorisationResult>? GetAuthorisationResultAsync<TInputPort>(TInputPort inputPort, ServiceFactory serviceFactory, CancellationToken cancellationToken)
        => inputPort is IInputPort<IAuthorisationOutputPort<TAuthorisationResult>>
            ? DelegateCache
                .GetFunction<(ServiceFactory, TInputPort, CancellationToken), Task<TAuthorisationResult>?>(
                    typeof(EnforcerCheckFactory<>).MakeGenericType(typeof(TAuthorisationResult), typeof(TInputPort)))?
                .Invoke((serviceFactory, inputPort, cancellationToken))
            : null;

    async Task IPipe.InvokeAsync<TInputPort, TOutputPort>(
        TInputPort inputPort,
        TOutputPort outputPort,
        ServiceFactory serviceFactory,
        NextPipeHandleAsync nextPipeHandle,
        CancellationToken cancellationToken)
    {
        if (outputPort is IAuthorisationOutputPort<TAuthorisationResult> _OutputPort)
        {
            var _AuthorisationResultAsync = GetAuthorisationResultAsync(inputPort, serviceFactory, cancellationToken);
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

        await nextPipeHandle().ConfigureAwait(false);
    }

    #endregion Methods

    #region - - - - - - Nested Classes - - - - - -

    private class EnforcerCheckFactory<TInputPort>
        : IDelegateProvider<(ServiceFactory, TInputPort, CancellationToken), Task<TAuthorisationResult>?>
        where TInputPort : IInputPort<IAuthorisationOutputPort<TAuthorisationResult>>
    {

        #region - - - - - - Methods - - - - - -

        public Func<(ServiceFactory, TInputPort, CancellationToken), Task<TAuthorisationResult>?> GetFunction()
            => tuple
                => tuple.Item1
                    .GetService<IAuthorisationEnforcer<TInputPort, TAuthorisationResult>>()?
                    .CheckAuthorisationAsync(tuple.Item2, tuple.Item3);

        #endregion Methods

    }

    #endregion Nested Classes

}
