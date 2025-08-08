using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Internal
{

    internal class AuthenticationPipe : IPipe
    {

        #region - - - - - - Methods - - - - - -

        Task IPipe.InvokeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            NextPipeHandleAsync nextPipeHandle,
            CancellationToken cancellationToken)
            => outputPort is IAuthenticationFailureOutputPort _OutputPort
                && serviceFactory.GetService<IPrincipalAccessor>()?.Principal == null
                ? _OutputPort.PresentAuthenticationFailureAsync(cancellationToken)
                : nextPipeHandle();

        #endregion Methods

    }

}
