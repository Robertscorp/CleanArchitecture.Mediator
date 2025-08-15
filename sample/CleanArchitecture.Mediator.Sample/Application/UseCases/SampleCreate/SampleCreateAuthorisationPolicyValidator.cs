namespace CleanArchitecture.Mediator.Sample.Application.UseCases.SampleCreate;

public class SampleCreateAuthorisationPolicyValidator : IAuthorisationPolicyValidator<SampleCreateInputPort, object>
{

    #region - - - - - - Methods - - - - - -

    Task<bool> IAuthorisationPolicyValidator<SampleCreateInputPort, object>.ValidateAsync(
        SampleCreateInputPort inputPort,
        out object policyFailure,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
    {
        policyFailure = new();

        return Task.FromResult(!inputPort.FailAuthorisation);
    }

    #endregion Methods

}
