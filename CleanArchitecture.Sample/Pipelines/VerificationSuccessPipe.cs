using CleanArchitecture.Mediator;
using CleanArchitecture.Sample.OutputPorts;

namespace CleanArchitecture.Sample.Pipelines
{

    public class VerificationSuccessPipe : IPipe
    {

        #region - - - - - - Methods - - - - - -

        Task IPipe.InvokeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            PipeHandle nextPipeHandle,
            CancellationToken cancellationToken)
            => outputPort is IVerificationSuccessOutputPort _OutputPort
                ? _OutputPort.PresentVerificationSuccessAsync(cancellationToken)
                : Task.CompletedTask;

        #endregion Methods

    }

}
