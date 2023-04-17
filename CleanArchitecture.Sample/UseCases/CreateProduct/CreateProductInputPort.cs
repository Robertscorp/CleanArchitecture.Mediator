using CleanArchitecture.Mediator;

namespace CleanArchitecture.Sample.UseCases.CreateProduct
{

    public class CreateProductInputPort : IInputPort<ICreateProductOutputPort>
    {

        #region - - - - - - Properties - - - - - -

        public bool FailAuthorisation { get; set; }

        public bool FailValidation { get; set; }

        #endregion Properties

    }

}
