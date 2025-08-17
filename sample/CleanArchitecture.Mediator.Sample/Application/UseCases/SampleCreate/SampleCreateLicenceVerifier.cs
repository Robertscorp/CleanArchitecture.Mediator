using CleanArchitecture.Mediator.Sample.Application.Services.Validation;

namespace CleanArchitecture.Mediator.Sample.Application.UseCases.SampleCreate;

public class SampleCreateLicenceVerifier : ILicencePolicyValidator<SampleCreateInputPort, SampleLicencePolicyFailure>
{

    #region - - - - - - Methods - - - - - -

    Task<bool> ILicencePolicyValidator<SampleCreateInputPort, SampleLicencePolicyFailure>.ValidateAsync(
        SampleCreateInputPort inputPort,
        out SampleLicencePolicyFailure policyFailure,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
    {
        policyFailure = new(inputPort.FailLicenceVerification ? "Failure" : string.Empty);

        return Task.FromResult(!inputPort.FailLicenceVerification);
    }

    #endregion Methods

}
