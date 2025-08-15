namespace CleanArchitecture.Mediator.Sample.Application.Services.OutputPorts;

public interface IVerificationSuccessOutputPort
{

    #region - - - - - - Methods - - - - - -

    Task PresentVerificationSuccessAsync(CancellationToken cancellationToken);

    #endregion Methods

}
