namespace CleanArchitecture.Sample.OutputPorts
{

    public interface IVerificationSuccessOutputPort
    {

        #region - - - - - - Methods - - - - - -

        Task PresentVerificationSuccessAsync(CancellationToken cancellationToken);

        #endregion Methods

    }

}
