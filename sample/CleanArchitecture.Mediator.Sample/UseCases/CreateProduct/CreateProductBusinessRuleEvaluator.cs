namespace CleanArchitecture.Mediator.Sample.UseCases.CreateProduct;

internal class CreateProductBusinessRuleEvaluator : IBusinessRuleEvaluator<CreateProductInputPort, ICreateProductOutputPort>
{

    #region - - - - - - Methods - - - - - -

    async Task<ContinuationBehaviour> IBusinessRuleEvaluator<CreateProductInputPort, ICreateProductOutputPort>.EvaluateAsync(
        CreateProductInputPort inputPort,
        ICreateProductOutputPort outputPort,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
    {
        var _Continuation = ContinuationBehaviour.Continue;

        if (inputPort.FailUniqueNameBusinessRule)
        {
            await outputPort.PresentNameMustBeUniqueAsync("SomeProduct", cancellationToken).ConfigureAwait(false);
            _Continuation = ContinuationBehaviour.Return;
        }

        if (inputPort.FailInvalidCategoryBusinessRule)
        {
            await outputPort.PresentCategoryDoesNotExistAsync(123, cancellationToken).ConfigureAwait(false);
            _Continuation = ContinuationBehaviour.Return;
        }

        return _Continuation;
    }

    #endregion Methods

}
