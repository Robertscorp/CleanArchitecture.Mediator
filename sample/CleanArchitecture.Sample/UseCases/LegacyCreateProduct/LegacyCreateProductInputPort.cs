using CleanArchitecture.Mediator;

namespace CleanArchitecture.Sample.UseCases.LegacyCreateProduct
{

    public class LegacyCreateProductInputPort : IInputPort<ILegacyCreateProductOutputPort>
    {

        #region - - - - - - Properties - - - - - -

        public bool FailAuthorisation { get; set; }

        public bool FailBusinessRuleValidation { get; set; }

        public bool FailInputPortValidation { get; set; }

        #endregion Properties

    }

}
