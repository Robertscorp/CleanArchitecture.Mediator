using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Pipes
{

    /// <summary>
    /// Handles invocation of the authorisation enforcer service.
    /// </summary>
    public class AuthorisationPipe : IPipe
    {

        #region - - - - - - Methods - - - - - -

        async Task IPipe.InvokeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            PipeHandle nextPipeHandle,
            CancellationToken cancellationToken)
        {
            var _AuthEnforcer = serviceFactory.GetService<IAuthorisationEnforcer<TInputPort, TOutputPort>>();
            if (_AuthEnforcer == null || await _AuthEnforcer.HandleAuthorisationAsync(inputPort, outputPort, cancellationToken).ConfigureAwait(false))
                await nextPipeHandle.InvokePipeAsync(inputPort, outputPort, serviceFactory, cancellationToken).ConfigureAwait(false);
        }

        #endregion Methods

    }

}
