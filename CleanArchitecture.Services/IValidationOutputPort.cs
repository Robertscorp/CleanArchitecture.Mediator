namespace CleanArchitecture.Services
{

    public interface IValidationOutputPort<TValidationFailure>
    {

        #region - - - - - - Methods - - - - - -

        Task PresentValidationFailureAsync(TValidationFailure validationFailure, CancellationToken cancellationToken);

        #endregion Methods

    }

}
