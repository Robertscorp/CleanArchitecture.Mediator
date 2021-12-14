using CleanArchitecture.Sample.Dtos;
using CleanArchitecture.Sample.Pipeline;
using CleanArchitecture.Services;

namespace CleanArchitecture.Sample.UseCases.CreateProduct
{

    public interface ICreateProductOutputPort :
        IAuthenticationOutputPort,
        IAuthorisationOutputPort<AuthorisationResult>,
        IBusinessRuleValidationOutputPort<ValidationResult>,
        IValidationOutputPort<ValidationResult>
    {

        #region - - - - - - Methods - - - - - -

        Task PresentCreatedProductAsync(ProductDto product, CancellationToken cancellationToken);

        #endregion Methods

    }

}
