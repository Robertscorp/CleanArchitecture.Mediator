using CleanArchitecture.Mediator.Sample.Application.Services.OutputPorts;
using CleanArchitecture.Mediator.Sample.Application.Services.Validation;
using CleanArchitecture.Mediator.Sample.Application.UseCases.SampleCreate;
using CleanArchitecture.Mediator.Sample.Domain.Entities;

namespace CleanArchitecture.Mediator.Sample.InterfaceAdapters.Presenters;

public class SampleCreatePresenter : ISampleCreateOutputPort, IVerificationSuccessOutputPort
{

    #region - - - - - - Properties - - - - - -

    public bool WarnOnInputPortValidationFailure { get; set; }

    #endregion Properties

    #region - - - - - - Methods - - - - - -

    Task IAuthenticationFailureOutputPort.PresentAuthenticationFailureAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("\t- SampleCreatePresenter.PresentAuthenticationFailureAsync");
        return Task.CompletedTask;
    }

    Task<ContinuationBehaviour> IAuthorisationPolicyFailureOutputPort<SampleAuthorisationPolicyFailure>.PresentAuthorisationPolicyFailureAsync(
        SampleAuthorisationPolicyFailure policyFailure,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("\t- SampleCreatePresenter.PresentAuthorisationPolicyFailureAsync");
        return ContinuationBehaviour.ReturnAsync;
    }

    Task<ContinuationBehaviour> ISampleCreateOutputPort.PresentCategoryDoesNotExistAsync(int categoryID, CancellationToken cancellationToken)
    {
        Console.WriteLine("\t- SampleCreatePresenter.PresentCategoryDoesNotExistAsync [Warn]");
        return ContinuationBehaviour.ContinueAsync;
    }

    Task ISampleCreateOutputPort.PresentCreatedSampleEntityAsync(SampleEntity sampleEntity, CancellationToken cancellationToken)
    {
        Console.WriteLine($"\t- SampleCreatePresenter.PresentCreatedSampleEntityAsync('{sampleEntity.Name}')");
        return Task.CompletedTask;
    }

    Task<ContinuationBehaviour> ISampleCreateOutputPort.PresentNameMustBeUniqueAsync(string name, CancellationToken cancellationToken)
    {
        Console.WriteLine($"\t- SampleCreatePresenter.PresentNameMustBeUniqueAsync [Fail]");
        return ContinuationBehaviour.ReturnAsync;
    }

    async Task<ContinuationBehaviour> IInputPortValidationFailureOutputPort<SampleInputPortValidationFailure>.PresentInputPortValidationFailureAsync(
        SampleInputPortValidationFailure validationFailure,
        CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken); // This only exists to showcase when to use the non-async ContinuationBehaviour fields.

        Console.WriteLine($"\t- SampleCreatePresenter.PresentInputPortValidationFailureAsync");

        return this.WarnOnInputPortValidationFailure
            ? ContinuationBehaviour.Continue
            : ContinuationBehaviour.Return;
    }

    Task<ContinuationBehaviour> ILicencePolicyFailureOutputPort<SampleLicencePolicyFailure>.PresentLicencePolicyFailureAsync(
        SampleLicencePolicyFailure policyFailure,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"\t- SampleCreatePresenter.PresentLicencePolicyFailureAsync");
        return ContinuationBehaviour.ReturnAsync;
    }

    Task IVerificationSuccessOutputPort.PresentVerificationSuccessAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("\t- SampleCreatePresenter.PresentVerificationSuccessAsync");
        return Task.CompletedTask;
    }

    #endregion Methods

}
