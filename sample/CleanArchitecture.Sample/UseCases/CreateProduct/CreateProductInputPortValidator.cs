﻿using CleanArchitecture.Mediator;
using CleanArchitecture.Sample.Pipelines;

namespace CleanArchitecture.Sample.UseCases.CreateProduct
{

    public class CreateProductInputPortValidator : IValidator<CreateProductInputPort, ValidationResult>
    {

        #region - - - - - - Methods - - - - - -

        Task<ValidationResult> IValidator<CreateProductInputPort, ValidationResult>.ValidateAsync(
            CreateProductInputPort inputPort,
            CancellationToken cancellationToken)
            => Task.FromResult(new ValidationResult { IsValid = !inputPort.FailValidation });

        #endregion Methods

    }

}
