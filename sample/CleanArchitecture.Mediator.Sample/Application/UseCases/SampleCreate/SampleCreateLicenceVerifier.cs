namespace CleanArchitecture.Mediator.Sample.Application.UseCases.SampleCreate;

public class SampleCreateLicenceVerifier : ILicencePolicyValidator<SampleCreateInputPort, object>
{

    #region - - - - - - Methods - - - - - -

    Task<bool> ILicencePolicyValidator<SampleCreateInputPort, object>.ValidateAsync(
        SampleCreateInputPort inputPort,
        out object policyFailure,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
    {
        policyFailure = new();

        return Task.FromResult(!inputPort.FailLicenceVerification);
    }

    #endregion Methods

}
