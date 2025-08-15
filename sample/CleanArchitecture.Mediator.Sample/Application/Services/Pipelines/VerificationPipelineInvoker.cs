using CleanArchitecture.Mediator.Sample.Application.Services.OutputPorts;

namespace CleanArchitecture.Mediator.Sample.Application.Services.Pipelines;

public class VerificationPipelineInvoker(VerificationPipeline pipeline)
{

    #region - - - - - - Methods - - - - - -

    public Task InvokeAsync<TOutputPort, TPresenter>(
        IInputPort<TOutputPort> inputPort,
        TPresenter presenter,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken) where TPresenter : IVerificationSuccessOutputPort, TOutputPort
        => pipeline.InvokeAsync(inputPort, presenter, serviceFactory, cancellationToken);

    #endregion Methods

}
