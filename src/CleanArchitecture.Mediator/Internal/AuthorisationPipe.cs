using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Internal
{

    internal class AuthorisationPipe<TInputPort, TOutputPort, TAuthorisationFailure> : IPipe<TInputPort, TOutputPort>
        where TInputPort : IInputPort<TOutputPort>, IInputPort<IAuthorisationOutputPort<TAuthorisationFailure>>
        where TOutputPort : IAuthorisationOutputPort<TAuthorisationFailure>
    {

        #region - - - - - - Methods - - - - - -

        async Task IPipe<TInputPort, TOutputPort>.InvokeAsync(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            NextPipeHandleAsync nextPipeHandle,
            CancellationToken cancellationToken)
        {
            var _AuthorisationEnforcer = serviceFactory.GetService<IAuthorisationEnforcer<TInputPort, TAuthorisationFailure>>();
            if (_AuthorisationEnforcer != null && !await _AuthorisationEnforcer.IsAuthorisedAsync(inputPort, out var _AuthorisationFailure, serviceFactory, cancellationToken).ConfigureAwait(false))
            {
                await outputPort.PresentAuthorisationFailureAsync(_AuthorisationFailure, cancellationToken).ConfigureAwait(false);
                return;
            }

            await nextPipeHandle().ConfigureAwait(false);
        }

        #endregion Methods

    }

}
