namespace CleanArchitecture.Services
{

    public interface IBusinessRuleValidationOutputPort<TValidationFailure>
    {

        #region - - - - - - Methods - - - - - -

        Task PresentBusinessRuleValidationFailureAsync(TValidationFailure validationFailure, CancellationToken cancellationToken);

        #endregion Methods

    }

}
