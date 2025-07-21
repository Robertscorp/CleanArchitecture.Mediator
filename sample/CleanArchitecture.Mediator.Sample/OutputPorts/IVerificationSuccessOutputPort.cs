namespace CleanArchitecture.Mediator.Sample.OutputPorts;

public interface IVerificationSuccessOutputPort
{

    #region - - - - - - Methods - - - - - -

    Task PresentVerificationSuccessAsync(CancellationToken cancellationToken);

    #endregion Methods

}
