namespace CleanArchitecture.Mediator.Sample.UseCases.CreateProduct;

public class CreateProductInputPort : IInputPort<ICreateProductOutputPort>
{

    #region - - - - - - Properties - - - - - -

    public bool FailAuthorisation { get; set; }

    public bool FailInputPortValidation { get; set; }

    public bool FailInvalidCategoryBusinessRule { get; set; }

    public bool FailLicenceVerification { get; set; }

    public bool FailUniqueNameBusinessRule { get; set; }

    #endregion Properties

}
