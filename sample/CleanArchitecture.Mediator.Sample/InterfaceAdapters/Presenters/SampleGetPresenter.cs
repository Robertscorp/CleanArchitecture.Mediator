using CleanArchitecture.Mediator.Sample.Application.Services.OutputPorts;
using CleanArchitecture.Mediator.Sample.Application.UseCases.SampleGet;
using CleanArchitecture.Mediator.Sample.Domain.Entities;

namespace CleanArchitecture.Mediator.Sample.InterfaceAdapters.Presenters;

public class SampleGetPresenter : ISampleGetOutputPort, IVerificationSuccessOutputPort
{

    #region - - - - - - Methods - - - - - -

    Task ISampleGetOutputPort.PresentSampleDataAsync(IQueryable<SampleEntity> sampleEntities, CancellationToken cancellationToken)
    {
        foreach (var _SampleEntity in sampleEntities)
            Console.WriteLine($"\t- SampleGetPresenter.PresentSampleDataAsync('{_SampleEntity.Name}')");

        return Task.CompletedTask;
    }

    Task IVerificationSuccessOutputPort.PresentVerificationSuccessAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"\t- SampleGetPresenter.PresentVerificationSuccessAsync");
        return Task.CompletedTask;
    }

    #endregion Methods

}
