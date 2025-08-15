namespace CleanArchitecture.Mediator.Sample.Application.UseCases.SampleCreate;

internal class SampleCreateInputPortValidator : IInputPortValidator<SampleCreateInputPort, object>
{

    #region - - - - - - Methods - - - - - -

    Task<bool> IInputPortValidator<SampleCreateInputPort, object>.ValidateAsync(
        SampleCreateInputPort inputPort,
        out object validationFailure,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
    {
        validationFailure = new();

        return Task.FromResult(!inputPort.FailInputPortValidation);
    }

    #endregion Methods

}
