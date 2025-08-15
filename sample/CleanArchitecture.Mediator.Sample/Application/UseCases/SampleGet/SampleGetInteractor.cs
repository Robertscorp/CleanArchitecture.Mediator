using CleanArchitecture.Mediator.Sample.Domain.Entities;

namespace CleanArchitecture.Mediator.Sample.Application.UseCases.SampleGet;

public class SampleGetInteractor : IInteractor<SampleGetInputPort, ISampleGetOutputPort>
{

    #region - - - - - - Methods - - - - - -

    Task IInteractor<SampleGetInputPort, ISampleGetOutputPort>.HandleAsync(
        SampleGetInputPort inputPort,
        ISampleGetOutputPort outputPort,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
        => outputPort.PresentSampleDataAsync(new List<SampleEntity>() { new() { Name = "Sample Entity Name" } }.AsQueryable(), cancellationToken);

    #endregion Methods

}
