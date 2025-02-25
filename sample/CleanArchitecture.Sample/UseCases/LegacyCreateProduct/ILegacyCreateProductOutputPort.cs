using CleanArchitecture.Mediator;
using CleanArchitecture.Sample.Dtos;
using CleanArchitecture.Sample.Legacy.Authorisation;
using CleanArchitecture.Sample.Legacy.BusinessRuleValidation;
using CleanArchitecture.Sample.Legacy.InputPortValidation;

namespace CleanArchitecture.Sample.UseCases.LegacyCreateProduct
{

    public interface ILegacyCreateProductOutputPort :
        IAuthenticationOutputPort,
        IAuthorisationOutputPort<AuthorisationResult>,
        IBusinessRuleValidationOutputPort<BusinessRuleValidationResult>,
        IInputPortValidationOutputPort<InputPortValidationResult>
    {

        #region - - - - - - Methods - - - - - -

        Task PresentCreatedProductAsync(ProductDto product, CancellationToken cancellationToken);

        #endregion Methods

    }

}
