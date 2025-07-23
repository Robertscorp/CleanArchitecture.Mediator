using CleanArchitecture.Mediator.Sample.OutputPorts;

namespace CleanArchitecture.Mediator.Sample.Pipelines;

public class VerificationSuccessPipe : IPipe
{

    #region - - - - - - Methods - - - - - -

    Task IPipe.InvokeAsync<TInputPort, TOutputPort>(
        TInputPort inputPort,
        TOutputPort outputPort,
        ServiceFactory serviceFactory,
        NextPipeHandleAsync nextPipeHandle,
        CancellationToken cancellationToken)
        => outputPort is IVerificationSuccessOutputPort _OutputPort
            ? _OutputPort.PresentVerificationSuccessAsync(cancellationToken)
            : Task.CompletedTask;

    #endregion Methods

}
