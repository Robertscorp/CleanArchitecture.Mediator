namespace CleanArchitecture.Services.Pipeline.Validation
{

    public interface IValidationOutputPort<TValidationFailure>
    {

        #region - - - - - - Methods - - - - - -

        Task PresentValidationFailureAsync(TValidationFailure validationFailure, CancellationToken cancellationToken);

        #endregion Methods

    }

}
