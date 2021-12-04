namespace CleanArchitecture.Services.Extended.Validation
{

    public interface IValidationOutputPort<TValidationResult>
    {

        #region - - - - - - Methods - - - - - -

        Task PresentValidationFailureAsync(TValidationResult validationResult, CancellationToken cancellationToken);

        #endregion Methods

    }

}
