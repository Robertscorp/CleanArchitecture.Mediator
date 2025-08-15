namespace CleanArchitecture.Mediator.Sample.Application.UseCases.SampleCreate;

public class SampleCreateInteractor : IInteractor<SampleCreateInputPort, ISampleCreateOutputPort>
{

    #region - - - - - - Methods - - - - - -

    Task IInteractor<SampleCreateInputPort, ISampleCreateOutputPort>.HandleAsync(
        SampleCreateInputPort inputPort,
        ISampleCreateOutputPort outputPort,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
        => outputPort.PresentCreatedSampleEntityAsync(new() { Name = "Created Sample Entity" }, cancellationToken);

    #endregion Methods

}
