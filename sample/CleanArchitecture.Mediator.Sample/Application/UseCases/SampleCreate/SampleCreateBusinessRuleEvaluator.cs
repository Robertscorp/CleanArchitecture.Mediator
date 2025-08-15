namespace CleanArchitecture.Mediator.Sample.Application.UseCases.SampleCreate;

internal class SampleCreateBusinessRuleEvaluator : IBusinessRuleEvaluator<SampleCreateInputPort, ISampleCreateOutputPort>
{

    #region - - - - - - Methods - - - - - -

    async Task<ContinuationBehaviour> IBusinessRuleEvaluator<SampleCreateInputPort, ISampleCreateOutputPort>.EvaluateAsync(
        SampleCreateInputPort inputPort,
        ISampleCreateOutputPort outputPort,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
    {
        var _Continuation = ContinuationBehaviour.Continue;

        if (inputPort.FailUniqueNameBusinessRule)
            _Continuation = _Continuation.AggregateWith(await outputPort.PresentNameMustBeUniqueAsync("Sample Entity Name", cancellationToken).ConfigureAwait(false));

        if (inputPort.FailInvalidCategoryBusinessRule)
            _Continuation = _Continuation.AggregateWith(await outputPort.PresentCategoryDoesNotExistAsync(123, cancellationToken).ConfigureAwait(false));

        return _Continuation;
    }

    #endregion Methods

}
