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
            _Continuation = _Continuation.AggregateWith(await outputPort.PresentNameMustBeUniqueAsync("SomeProduct", cancellationToken).ConfigureAwait(false));

        if (inputPort.FailInvalidCategoryBusinessRule)
            _Continuation = _Continuation.AggregateWith(await outputPort.PresentCategoryDoesNotExistAsync(123, cancellationToken).ConfigureAwait(false));

        return _Continuation;
    }

    #endregion Methods

}
