namespace CleanArchitecture.Mediator.Sample.Application.UseCases.SampleCreate;

public class SampleCreateInputPort : IInputPort<ISampleCreateOutputPort>
{

    #region - - - - - - Properties - - - - - -

    public bool FailAuthorisation { get; set; }

    public bool FailInputPortValidation { get; set; }

    public bool FailInvalidCategoryBusinessRule { get; set; }

    public bool FailLicenceVerification { get; set; }

    public bool FailUniqueNameBusinessRule { get; set; }

    #endregion Properties

}
