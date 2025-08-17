namespace CleanArchitecture.Mediator.Sample.Application.Services.Validation;

public class SampleAuthorisationPolicyFailure(string failure)
{

    #region - - - - - - Properties - - - - - -

    public string Failure { get; } = failure;

    #endregion Properties

}
