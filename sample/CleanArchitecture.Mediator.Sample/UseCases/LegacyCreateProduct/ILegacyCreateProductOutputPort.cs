using CleanArchitecture.Mediator.Sample.Dtos;
using CleanArchitecture.Mediator.Sample.Legacy.BusinessRuleValidation;

namespace CleanArchitecture.Mediator.Sample.UseCases.LegacyCreateProduct;

public interface ILegacyCreateProductOutputPort :
    IAuthenticationOutputPort,
    IAuthorisationOutputPort<object>,
    IBusinessRuleValidationOutputPort<BusinessRuleValidationResult>,
    IInputPortValidationOutputPort<object>
{

    #region - - - - - - Methods - - - - - -

    Task PresentCreatedProductAsync(ProductDto product, CancellationToken cancellationToken);

    #endregion Methods

}
