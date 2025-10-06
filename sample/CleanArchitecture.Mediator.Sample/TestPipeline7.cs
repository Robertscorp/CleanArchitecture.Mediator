using CleanArchitecture.Mediator.Sample.Application.Services.Validation;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace CleanArchitecture.Mediator.Sample;

/// <summary>
/// The current implementation of this doesn't allow doing work before and after the pipe logic, because the next pipe isn't passed in as a delegate
/// In theory, we can use "NextPipeHandleAsync" as a placeholder and replace it with the actual method when we do code gen.
/// </summary>
public static partial class TestPipeline7
{

    private delegate Task PipeHandleAsync<TOutputPort>(object inputPort, TOutputPort outputPort, ServiceFactory serviceFactory, CancellationToken cancellationToken);

    private static readonly ConcurrentDictionary<Type, PipeHandleAsync<object>> s_PipelineHandleCache = [];

    private static readonly MethodInfo s_InvokeAuthenticationPipeAsyncMethodInfo = typeof(TestPipeline7).GetMethod(nameof(InvokeAuthenticationPipeAsync), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo s_InvokeAuthorisationPipeAsyncMethodInfo = typeof(TestPipeline7).GetMethod(nameof(InvokeAuthorisationPipeAsync), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo s_InvokeInputPortValidationPipeAsyncMethodInfo = typeof(TestPipeline7).GetMethod(nameof(InvokeInputPortValidationPipeAsync), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo s_InvokeValidationPipeAsyncMethodInfo = typeof(TestPipeline7).GetMethod(nameof(InvokeValidationPipeAsync), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo s_InvokeInteractorPipeAsyncMethodInfo = typeof(TestPipeline7).GetMethod(nameof(InvokeInteractorInvocationPipeAsync), BindingFlags.NonPublic | BindingFlags.Static)!;

    private static PipeHandleAsync<object> BuildPipeline(Type inputType, Type outputType)
    {
        var _Stages = new List<Func<object, object, ServiceFactory, CancellationToken, Task<ContinuationBehaviour>>>();

        if (outputType.IsAssignableTo(typeof(IAuthenticationFailureOutputPort)))
            _Stages.Add(new(GetPipeDelegate(inputType, outputType, s_InvokeAuthenticationPipeAsyncMethodInfo)));

        if (inputType.IsAssignableTo(typeof(IInputPort<IAuthorisationPolicyFailureOutputPort<SampleInputPortValidationFailure>>))
            && outputType.IsAssignableTo(typeof(IAuthorisationPolicyFailureOutputPort<SampleInputPortValidationFailure>)))
            _Stages.Add(new(GetPipeDelegate(inputType, outputType, s_InvokeAuthorisationPipeAsyncMethodInfo)));

        if (inputType.IsAssignableTo(typeof(IInputPort<IInputPortValidationFailureOutputPort<SampleInputPortValidationFailure>>))
            && outputType.IsAssignableTo(typeof(IInputPortValidationFailureOutputPort<SampleInputPortValidationFailure>)))
            _Stages.Add(new(GetPipeDelegate(inputType, outputType, s_InvokeInputPortValidationPipeAsyncMethodInfo)));

        _Stages.Add(new(GetPipeDelegate(inputType, outputType, s_InvokeValidationPipeAsyncMethodInfo)));
        _Stages.Add(new(GetPipeDelegate(inputType, outputType, s_InvokeInteractorPipeAsyncMethodInfo)));

        var _StageArray = _Stages.ToArray();

        return new PipeHandleAsync<object>(async (input, output, factory, ct) =>
        {
            var _CachedStageArray = _StageArray;
            var _Continuation = default(ContinuationBehaviour);

            for (var _Index = 0; _Index < _CachedStageArray.Length; _Index++)
                if (_Continuation is null)
                    _Continuation = await _CachedStageArray[_Index](input, output, factory, ct);

                else
                    await _Continuation.HandleAsync(new(async () => _Continuation = await _CachedStageArray[_Index](input, output, factory, ct)), ct);
        });
    }

    private static Func<object, object, ServiceFactory, CancellationToken, Task<ContinuationBehaviour>> GetPipeDelegate(Type inputType, Type outputType, MethodInfo genericMethod)
    {
        var _MethodInfo = genericMethod.MakeGenericMethod(inputType, outputType);
        var _Input = Expression.Parameter(typeof(object));
        var _Output = Expression.Parameter(typeof(object));
        var _Factory = Expression.Parameter(typeof(ServiceFactory));
        var _Token = Expression.Parameter(typeof(CancellationToken));

        var _Call = Expression.Call(
            null,
            _MethodInfo,
            Expression.Convert(_Input, inputType),
            Expression.Convert(_Output, outputType),
            _Factory,
            _Token);

        return Expression
            .Lambda<Func<object, object, ServiceFactory, CancellationToken, Task<ContinuationBehaviour>>>(
                _Call, _Input, _Output, _Factory, _Token)
            .Compile();
    }

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

            _Value = BuildPipeline(_InputPortType, _OutputPortType);

            s_PipelineHandleCache[_InputPortType] = _Value;
        }

        return _Value(inputPort, outputPort, serviceFactory, cancellationToken);
    }

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

}