using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Pipes
{

    /// <summary>
    /// Handles invocation of the validator service.
    /// </summary>
    public class ValidationPipe : IPipe
    {

        #region - - - - - - Methods - - - - - -

        async Task IPipe.InvokeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            PipeHandle nextPipeHandle,
            CancellationToken cancellationToken)
        {
            var _Validator = serviceFactory.GetService<IValidator<TInputPort, TOutputPort>>();
            if (_Validator == null || await _Validator.HandleValidationAsync(inputPort, outputPort, cancellationToken).ConfigureAwait(false))
                await nextPipeHandle.InvokePipeAsync(inputPort, outputPort, serviceFactory, cancellationToken).ConfigureAwait(false);
        }

        #endregion Methods

    }

}
