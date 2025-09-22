using CleanArchitecture.Mediator.Sample.Application.Services.Validation;
using System.Collections.Concurrent;
using System.Reflection;

namespace CleanArchitecture.Mediator.Sample;

internal partial class TestPipeline5
{

    private delegate Task PipeHandleAsync<TOutputPort>(object inputPort, TOutputPort outputPort, ServiceFactory serviceFactory, CancellationToken cancellationToken);

    private static readonly MethodInfo s_InvokeAuthenticationPipeAsyncMethodInfo = typeof(TestPipeline5).GetMethod(nameof(InvokeAuthenticationPipeAsync), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo s_InvokeAuthorisationPipeAsyncMethodInfo = typeof(TestPipeline5).GetMethod(nameof(InvokeAuthorisationPipeAsync), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo s_InvokeInputPortValidationPipeAsyncMethodInfo = typeof(TestPipeline5).GetMethod(nameof(InvokeInputPortValidationPipeAsync), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo s_InvokeValidationPipeAsyncMethodInfo = typeof(TestPipeline5).GetMethod(nameof(InvokeValidationPipeWrapperAsync), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo s_InvokeInteractorPipeAsyncMethodInfo = typeof(TestPipeline5).GetMethod(nameof(InvokeInteractorInvocationPipeWrapperAsync), BindingFlags.NonPublic | BindingFlags.Static)!;

    private static readonly ConcurrentDictionary<Type, PipeHandleAsync<object>> s_PipelineHandleCache = [];

    /// <summary>
    /// Invokes the pipeline.
    /// </summary>
    /// <typeparam name="TOutputPort">The type of output port.</typeparam>
    /// <param name="inputPort">The input to the pipeline.</param>
    /// <param name="outputPort">The output mechanism for the pipeline.</param>
    /// <param name="serviceFactory">The factory used to get service instances.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
    /// <exception cref="ArgumentNullException"><paramref name="inputPort"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="outputPort"/> is null.</exception>
    public static Task InvokeAsync<TOutputPort>(
        IInputPort<TOutputPort> inputPort,
        TOutputPort outputPort,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
    {
        if (inputPort == null) throw new ArgumentNullException(nameof(inputPort));
        if (outputPort == null) throw new ArgumentNullException(nameof(outputPort));

        if (!s_PipelineHandleCache.TryGetValue(inputPort.GetType(), out var _Value))
        {
            var _InputPortType = inputPort.GetType();
            var _OutputPortType = typeof(TOutputPort);

            var _PipeHandle = new PipeHandleAsync<TOutputPort>(static (inputPort, outputPort, serviceFactory, cancellationToken) => Task.CompletedTask);
            _PipeHandle = GetPipeHandle(_InputPortType, s_InvokeInteractorPipeAsyncMethodInfo, _PipeHandle);
            _PipeHandle = GetPipeHandle(_InputPortType, s_InvokeValidationPipeAsyncMethodInfo, _PipeHandle);

            //if (_InputPortType.IsAssignableTo(typeof(IInputPort<IInputPortValidationFailureOutputPort<SampleInputPortValidationFailure>>))
            //    && _OutputPortType.IsAssignableTo(typeof(IInputPortValidationFailureOutputPort<SampleInputPortValidationFailure>)))
            //    _PipeHandle = GetPipeHandle(_InputPortType, s_InvokeInputPortValidationPipeAsyncMethodInfo, _PipeHandle);

            //if (_InputPortType.IsAssignableTo(typeof(IInputPort<IAuthorisationPolicyFailureOutputPort<SampleInputPortValidationFailure>>))
            //    && _OutputPortType.IsAssignableTo(typeof(IAuthorisationPolicyFailureOutputPort<SampleInputPortValidationFailure>)))
            //    _PipeHandle = GetPipeHandle(_InputPortType, s_InvokeAuthorisationPipeAsyncMethodInfo, _PipeHandle);

            //if (_OutputPortType.IsAssignableTo(typeof(IAuthenticationFailureOutputPort)))
            //    _PipeHandle = GetPipeHandle(_InputPortType, s_InvokeAuthenticationPipeAsyncMethodInfo, _PipeHandle);

            _Value = new((inputPort, outputPort, serviceFactory, cancellationToken) => _PipeHandle(inputPort, (TOutputPort)outputPort, serviceFactory, cancellationToken));

            s_PipelineHandleCache[_InputPortType] = _Value;
        }

        return _Value(inputPort, outputPort, serviceFactory, cancellationToken);
    }

    private static PipeHandleAsync<TOutputPort> GetPipeHandle<TOutputPort>(Type inputPortType, MethodInfo methodInfo, PipeHandleAsync<TOutputPort> nextPipe)
    {
        var _InvokeWrapper = (Func<object, TOutputPort, ServiceFactory, CancellationToken, Task<ContinuationBehaviour>>)Delegate.CreateDelegate(
            typeof(Func<object, TOutputPort, ServiceFactory, CancellationToken, Task<ContinuationBehaviour>>),
            methodInfo.MakeGenericMethod(inputPortType, typeof(TOutputPort)));

        return new(async (inputPort, outputPort, serviceFactory, cancellationToken) =>
        {
            var _Continuation = await _InvokeWrapper(inputPort, outputPort, serviceFactory, cancellationToken).ConfigureAwait(false);
            await _Continuation.HandleAsync(new(() => nextPipe.Invoke(inputPort, outputPort, serviceFactory, cancellationToken)), cancellationToken).ConfigureAwait(false);
        });
    }

    private static Task<ContinuationBehaviour> InvokeInteractorInvocationPipeWrapperAsync<TInputPort, TOutputPort>(
        object inputPort, TOutputPort outputPort, ServiceFactory factory, CancellationToken token)
        where TInputPort : IInputPort<TOutputPort>
        => InvokeInteractorInvocationPipeAsync((TInputPort)inputPort, outputPort, factory, token);

    private static async Task<ContinuationBehaviour> InvokeAuthenticationPipeAsync<TInputPort, TOutputPort>(
        TInputPort inputPort,
        TOutputPort outputPort,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
        where TOutputPort : IAuthenticationFailureOutputPort
    {
        if (serviceFactory.GetService<IPrincipalAccessor>().Principal == null)
        {
            await outputPort.PresentAuthenticationFailureAsync(cancellationToken);
            return ContinuationBehaviour.Return;
        }

        return ContinuationBehaviour.Continue;
    }

    private static async Task<ContinuationBehaviour> InvokeAuthorisationPipeAsync<TInputPort, TOutputPort>(
        TInputPort inputPort,
        TOutputPort outputPort,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
        where TInputPort : IInputPort<IAuthorisationPolicyFailureOutputPort<SampleAuthorisationPolicyFailure>>
        where TOutputPort : IAuthorisationPolicyFailureOutputPort<SampleAuthorisationPolicyFailure>
    {
        var _Validator = serviceFactory.GetService<IAuthorisationPolicyValidator<TInputPort, SampleAuthorisationPolicyFailure>>();
        return !await _Validator.ValidateAsync(inputPort, out var _PolicyFailure, serviceFactory, cancellationToken).ConfigureAwait(false)
            ? await outputPort.PresentAuthorisationPolicyFailureAsync(_PolicyFailure, cancellationToken).ConfigureAwait(false)
            : ContinuationBehaviour.Continue;
    }

    private static async Task<ContinuationBehaviour> InvokeInputPortValidationPipeAsync<TInputPort, TOutputPort>(
        TInputPort inputPort,
        TOutputPort outputPort,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
        where TInputPort : IInputPort<IInputPortValidationFailureOutputPort<SampleInputPortValidationFailure>>
        where TOutputPort : IInputPortValidationFailureOutputPort<SampleInputPortValidationFailure>
    {
        var _Validator = serviceFactory.GetService<IInputPortValidator<TInputPort, SampleInputPortValidationFailure>>();
        return !await _Validator.ValidateAsync(inputPort, out var _ValidationFailure, serviceFactory, cancellationToken).ConfigureAwait(false)
            ? await outputPort.PresentInputPortValidationFailureAsync(_ValidationFailure, cancellationToken).ConfigureAwait(false)
            : ContinuationBehaviour.Continue;
    }

    private static async Task<ContinuationBehaviour> InvokeInteractorInvocationPipeAsync<TInputPort, TOutputPort>(
        TInputPort inputPort,
        TOutputPort outputPort,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
        where TInputPort : IInputPort<TOutputPort>
    {
        await serviceFactory
            .GetService<IInteractor<TInputPort, TOutputPort>>()
            .HandleAsync(inputPort, outputPort, serviceFactory, cancellationToken);

        return ContinuationBehaviour.Return;
    }

    private static Task<ContinuationBehaviour> InvokeValidationPipeAsync<TInputPort, TOutputPort>(
        TInputPort inputPort,
        TOutputPort outputPort,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
        where TInputPort : IInputPort<TOutputPort>
    {
        var _Evaluator = serviceFactory.GetService<IBusinessRuleEvaluator<TInputPort, TOutputPort>>();
        return _Evaluator == null
            ? ContinuationBehaviour.ContinueAsync
            : _Evaluator.EvaluateAsync(inputPort, outputPort, serviceFactory, cancellationToken);
    }

    private static Task<ContinuationBehaviour> InvokeValidationPipeWrapperAsync<TInputPort, TOutputPort>(
        object inputPort, TOutputPort outputPort, ServiceFactory factory, CancellationToken token)
        where TInputPort : IInputPort<TOutputPort>
        => InvokeValidationPipeAsync((TInputPort)inputPort, outputPort, factory, token);

}
