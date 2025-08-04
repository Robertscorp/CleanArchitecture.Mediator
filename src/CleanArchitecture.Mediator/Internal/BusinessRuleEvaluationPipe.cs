using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Internal
{

    internal class BusinessRuleEvaluationPipe : IPipe
    {

        #region - - - - - - Methods - - - - - -

        async Task IPipe.InvokeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            NextPipeHandleAsync nextPipeHandle,
            CancellationToken cancellationToken)
        {
            var _Evaluator = serviceFactory.GetService<IBusinessRuleEvaluator<TInputPort, TOutputPort>>();
            if (_Evaluator == null)
            {
                await nextPipeHandle().ConfigureAwait(false);
                return;
            }

            var _Continuation = await _Evaluator.EvaluateAsync(inputPort, outputPort, serviceFactory, cancellationToken).ConfigureAwait(false);
            await _Continuation.HandleAsync(nextPipeHandle, cancellationToken).ConfigureAwait(false);
        }

        #endregion Methods

    }

}
