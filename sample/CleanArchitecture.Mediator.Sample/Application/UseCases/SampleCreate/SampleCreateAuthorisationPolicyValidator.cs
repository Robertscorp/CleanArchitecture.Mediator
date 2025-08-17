using CleanArchitecture.Mediator.Sample.Application.Services.Validation;

namespace CleanArchitecture.Mediator.Sample.Application.UseCases.SampleCreate;

public class SampleCreateAuthorisationPolicyValidator : IAuthorisationPolicyValidator<SampleCreateInputPort, SampleAuthorisationPolicyFailure>
{

    #region - - - - - - Methods - - - - - -

    Task<bool> IAuthorisationPolicyValidator<SampleCreateInputPort, SampleAuthorisationPolicyFailure>.ValidateAsync(
        SampleCreateInputPort inputPort,
        out SampleAuthorisationPolicyFailure policyFailure,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
    {
        policyFailure = new(inputPort.FailAuthorisation ? "Failure" : string.Empty);

        return Task.FromResult(!inputPort.FailAuthorisation);
    }

    #endregion Methods

}
