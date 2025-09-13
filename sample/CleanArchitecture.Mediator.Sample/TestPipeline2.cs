using CleanArchitecture.Mediator.Sample.Application.Services.Validation;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace CleanArchitecture.Mediator.Sample;

internal partial class TestPipeline2
{

    private static readonly MethodInfo s_InvokeMethodInfo = typeof(TestPipeline2).GetMethod(nameof(InvokeInternalAsync), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo s_InvokeAuthorisationPipeAsyncMethodInfo = typeof(TestPipeline2).GetMethod(nameof(InvokeAuthorisationPipeAsync), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo s_InvokeInputPortValidationPipeAsyncMethodInfo = typeof(TestPipeline2).GetMethod(nameof(InvokeInputPortValidationPipeAsync), BindingFlags.NonPublic | BindingFlags.Static)!;

    private static readonly ConcurrentDictionary<Type, object> s_InvokeHandleCache = [];
    private static readonly ConcurrentDictionary<Type, object> s_AuthorisationPipeHandleCache = [];
    private static readonly ConcurrentDictionary<Type, object> s_InputPortValidationPipeHandleCache = [];

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

        return ((Func<object, TOutputPort, ServiceFactory, CancellationToken, Task>)s_InvokeHandleCache
            .GetOrAdd(inputPort.GetType(), key => GetFunc<object, TOutputPort>(inputPort.GetType(), s_InvokeMethodInfo)))
            .Invoke(inputPort, outputPort, serviceFactory, cancellationToken);
    }

    private static async Task InvokeInternalAsync<TInputPort, TOutputPort>(
        TInputPort inputPort,
        TOutputPort outputPort,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
        where TInputPort : IInputPort<TOutputPort>
    {
        var _InputPortType = typeof(TInputPort);

        // Authentication Pipe
        _ = await InvokeAuthenticationPipeAsync(inputPort, outputPort, serviceFactory, cancellationToken).ConfigureAwait(false);

        // Authorisation Pipe
        if (inputPort is IInputPort<IAuthorisationPolicyFailureOutputPort<SampleInputPortValidationFailure>> &&
            outputPort is IAuthorisationPolicyFailureOutputPort<SampleInputPortValidationFailure>)
            await ((Func<TInputPort, TOutputPort, ServiceFactory, CancellationToken, Task>)s_AuthorisationPipeHandleCache
                .GetOrAdd(
                    _InputPortType,
                    key => GetFunc<TInputPort, TOutputPort>(_InputPortType, s_InvokeAuthorisationPipeAsyncMethodInfo)))
                .Invoke(inputPort, outputPort, serviceFactory, cancellationToken)
                .ConfigureAwait(false);

        // Input Port Validation Pipe
        if (inputPort is IInputPort<IInputPortValidationFailureOutputPort<SampleInputPortValidationFailure>> &&
            outputPort is IInputPortValidationFailureOutputPort<SampleInputPortValidationFailure>)
            await ((Func<object, TOutputPort, ServiceFactory, CancellationToken, Task>)s_InputPortValidationPipeHandleCache
                .GetOrAdd(
                    _InputPortType,
                    key => GetFunc<TInputPort, TOutputPort>(_InputPortType, s_InvokeInputPortValidationPipeAsyncMethodInfo)))
                .Invoke(inputPort, outputPort, serviceFactory, cancellationToken)
                .ConfigureAwait(false);

        // Validation Pipe
        _ = await InvokeValidationPipeAsync(inputPort, outputPort, serviceFactory, cancellationToken).ConfigureAwait(false);

        // Interactor Pipe
        _ = await InvokeInteractorInvocationPipeAsync(inputPort, outputPort, serviceFactory, cancellationToken).ConfigureAwait(false);
    }

    private static Func<TInputPort, TOutputPort, ServiceFactory, CancellationToken, Task> GetFunc<TInputPort, TOutputPort>(Type inputPortType, MethodInfo methodInfo)
    {
        var _InputPort = Expression.Parameter(typeof(TInputPort));
        var _OutputPort = Expression.Parameter(typeof(TOutputPort));
        var _ServiceFactory = Expression.Parameter(typeof(ServiceFactory));
        var _CancellationToken = Expression.Parameter(typeof(CancellationToken));

        return Expression
            .Lambda<Func<TInputPort, TOutputPort, ServiceFactory, CancellationToken, Task>>(
                Expression.Call(
                    null,
                    methodInfo.MakeGenericMethod(inputPortType, typeof(TOutputPort)),
                    [Expression.Convert(_InputPort, inputPortType), _OutputPort, _ServiceFactory, _CancellationToken]),
                [_InputPort, _OutputPort, _ServiceFactory, _CancellationToken])
            .Compile();
    }

    private static async Task<ContinuationBehaviour> InvokeAuthenticationPipeAsync<TInputPort, TOutputPort>(
        TInputPort inputPort,
        TOutputPort outputPort,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
    {
        if (outputPort is IAuthenticationFailureOutputPort _OutputPort
            && serviceFactory.GetService<IPrincipalAccessor>().Principal == null)
        {
            await _OutputPort.PresentAuthenticationFailureAsync(cancellationToken);
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
