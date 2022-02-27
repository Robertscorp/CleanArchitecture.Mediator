using CleanArchitecture.Mediator;

namespace CleanArchitecture.Sample.UseCases.CreateProduct
{

    public class CreateProductInputPort : IUseCaseInputPort<ICreateProductOutputPort>
    {

        #region - - - - - - Properties - - - - - -

        public bool FailAuthorisation { get; set; }

        public bool FailBusinessRuleValidation { get; set; }

        public bool FailInputPortValidation { get; set; }

        #endregion Properties

    }

}
