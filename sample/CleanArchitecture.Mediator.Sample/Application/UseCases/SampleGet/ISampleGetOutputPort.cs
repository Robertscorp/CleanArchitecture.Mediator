using CleanArchitecture.Mediator.Sample.Domain.Entities;

namespace CleanArchitecture.Mediator.Sample.Application.UseCases.SampleGet;

public interface ISampleGetOutputPort
{

    #region - - - - - - Methods - - - - - -

    Task PresentSampleDataAsync(IQueryable<SampleEntity> sampleEntities, CancellationToken cancellationToken);

    #endregion Methods

}
