﻿using CleanArchitecture.Mediator;
using CleanArchitecture.Sample.Dtos;
using CleanArchitecture.Sample.Pipelines;

namespace CleanArchitecture.Sample.UseCases.CreateProduct
{

    public interface ICreateProductOutputPort :
        IAuthenticationOutputPort,
        IAuthorisationOutputPort<AuthorisationResult>,
        IValidationOutputPort<ValidationResult>
    {

        #region - - - - - - Methods - - - - - -

        Task PresentCreatedProductAsync(ProductDto product, CancellationToken cancellationToken);

        #endregion Methods

    }

}
