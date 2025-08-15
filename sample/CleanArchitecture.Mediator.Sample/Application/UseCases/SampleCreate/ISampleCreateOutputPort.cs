using CleanArchitecture.Mediator.Sample.Domain.Entities;

namespace CleanArchitecture.Mediator.Sample.Application.UseCases.SampleCreate;

public interface ISampleCreateOutputPort :
    IAuthenticationFailureOutputPort,
    IAuthorisationPolicyFailureOutputPort<object>,
    IInputPortValidationFailureOutputPort<object>,
    ILicencePolicyFailureOutputPort<object>
{

    #region - - - - - - Methods - - - - - -

    Task<ContinuationBehaviour> PresentCategoryDoesNotExistAsync(int categoryID, CancellationToken cancellationToken);

    Task PresentCreatedSampleEntityAsync(SampleEntity sampleEntity, CancellationToken cancellationToken);

    Task<ContinuationBehaviour> PresentNameMustBeUniqueAsync(string name, CancellationToken cancellationToken);

    #endregion Methods

}
