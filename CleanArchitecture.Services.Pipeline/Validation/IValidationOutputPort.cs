namespace CleanArchitecture.Services.Pipeline.Validation
{

    public interface IValidationOutputPort<TValidationResult>
    {

        #region - - - - - - Methods - - - - - -

        Task PresentValidationFailureAsync(TValidationResult validationResult, CancellationToken cancellationToken);

        #endregion Methods

    }

}
