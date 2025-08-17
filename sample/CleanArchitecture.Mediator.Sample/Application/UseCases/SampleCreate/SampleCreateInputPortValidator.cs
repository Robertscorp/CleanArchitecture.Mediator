using CleanArchitecture.Mediator.Sample.Application.Services.Validation;

namespace CleanArchitecture.Mediator.Sample.Application.UseCases.SampleCreate;

internal class SampleCreateInputPortValidator : IInputPortValidator<SampleCreateInputPort, SampleInputPortValidationFailure>
{

    #region - - - - - - Methods - - - - - -

    Task<bool> IInputPortValidator<SampleCreateInputPort, SampleInputPortValidationFailure>.ValidateAsync(
        SampleCreateInputPort inputPort,
        out SampleInputPortValidationFailure validationFailure,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
    {
        validationFailure = new(inputPort.FailInputPortValidation ? [new("PropertyName", "Error")] : []);

        return Task.FromResult(!inputPort.FailInputPortValidation);
    }

    #endregion Methods

}
