using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Internal
{

    internal class AuthorisationPipe : IPipe
    {

        #region - - - - - - Methods - - - - - -

        async Task IPipe.InvokeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            IPipeHandle nextPipeHandle,
            CancellationToken cancellationToken)
        {
            var _AuthEnforcer = serviceFactory.GetService<IAuthorisationEnforcer<TInputPort, TOutputPort>>();
            if (_AuthEnforcer == null || await _AuthEnforcer.HandleAuthorisationAsync(inputPort, outputPort, serviceFactory, cancellationToken).ConfigureAwait(false))
                await nextPipeHandle.InvokePipeAsync(inputPort, outputPort, serviceFactory, cancellationToken).ConfigureAwait(false);
        }

        #endregion Methods

    }

}
