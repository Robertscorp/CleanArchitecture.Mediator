using CleanArchitecture.Mediator.Authentication;
using CleanArchitecture.Mediator.Pipeline;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Pipes
{

    /// <summary>
    /// Handles authentication verification and presenting authentication failures.
    /// </summary>
    public class AuthenticationPipe : IPipe
    {

        #region - - - - - - Methods - - - - - -

        Task IPipe.InvokeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            PipeHandle nextPipeHandle,
            CancellationToken cancellationToken)
            => outputPort is IAuthenticationOutputPort _OutputPort
                && serviceFactory.GetService<IAuthenticatedClaimsPrincipalProvider>()?.AuthenticatedClaimsPrincipal == null
                ? _OutputPort.PresentUnauthenticatedAsync(cancellationToken)
                : nextPipeHandle.InvokePipeAsync(inputPort, outputPort, serviceFactory, cancellationToken);

        #endregion Methods

    }

}
