using CleanArchitecture.Mediator.Sample.Application.Services.Validation;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace CleanArchitecture.Mediator.Sample;

internal partial class TestPipeline4
{

    private static readonly MethodInfo s_InvokeAuthenticationPipeAsyncMethodInfo = typeof(TestPipeline4).GetMethod(nameof(InvokeAuthenticationPipeAsync), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo s_InvokeAuthorisationPipeAsyncMethodInfo = typeof(TestPipeline4).GetMethod(nameof(InvokeAuthorisationPipeAsync), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo s_InvokeInputPortValidationPipeAsyncMethodInfo = typeof(TestPipeline4).GetMethod(nameof(InvokeInputPortValidationPipeAsync), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo s_InvokeValidationPipeAsyncMethodInfo = typeof(TestPipeline4).GetMethod(nameof(InvokeValidationPipeAsync), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo s_InvokeInteractorPipeAsyncMethodInfo = typeof(TestPipeline4).GetMethod(nameof(InvokeInteractorInvocationPipeAsync), BindingFlags.NonPublic | BindingFlags.Static)!;

    private static readonly ConcurrentDictionary<Type, object> s_InvokeHandleCache = [];

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

        if (!s_InvokeHandleCache.TryGetValue(inputPort.GetType(), out var _Value))
        {
            var _InputPortType = inputPort.GetType();
            var _AuthenticationPipeHandle = GetFunc<TOutputPort>(_InputPortType, s_InvokeAuthenticationPipeAsyncMethodInfo);

            var _AuthorisationPipeHandle = default(Func<object, TOutputPort, ServiceFactory, CancellationToken, Task>);
            if (inputPort is IInputPort<IAuthorisationPolicyFailureOutputPort<SampleInputPortValidationFailure>>
                && outputPort is IAuthorisationPolicyFailureOutputPort<SampleInputPortValidationFailure>)
                _AuthorisationPipeHandle = GetFunc<TOutputPort>(_InputPortType, s_InvokeAuthorisationPipeAsyncMethodInfo);

            var _InputPortValidationPipeHandle = default(Func<object, TOutputPort, ServiceFactory, CancellationToken, Task>);
            if (inputPort is IInputPort<IInputPortValidationFailureOutputPort<SampleInputPortValidationFailure>>
                && outputPort is IInputPortValidationFailureOutputPort<SampleInputPortValidationFailure>)
                _InputPortValidationPipeHandle = GetFunc<TOutputPort>(_InputPortType, s_InvokeInputPortValidationPipeAsyncMethodInfo);

            var _ValidationPipeHandle = GetFunc<TOutputPort>(_InputPortType, s_InvokeValidationPipeAsyncMethodInfo);

            var _InteractorPipeHandle = GetFunc<TOutputPort>(_InputPortType, s_InvokeInteractorPipeAsyncMethodInfo);

            _Value = async (object inputPort, TOutputPort outputPort, ServiceFactory serviceFactory, CancellationToken cancellationToken) =>
            {
                // Authentication Pipe
                await _AuthenticationPipeHandle.Invoke(inputPort, outputPort, serviceFactory, cancellationToken).ConfigureAwait(false);

                // Authorisation Pipe
                if (_AuthorisationPipeHandle != null)
                    await _AuthorisationPipeHandle.Invoke(inputPort, outputPort, serviceFactory, cancellationToken).ConfigureAwait(false);

                // Input Port Validation Pipe
                if (_InputPortValidationPipeHandle != null)
                    await _InputPortValidationPipeHandle.Invoke(inputPort, outputPort, serviceFactory, cancellationToken).ConfigureAwait(false);

                // Validation Pipe
                await _ValidationPipeHandle.Invoke(inputPort, outputPort, serviceFactory, cancellationToken).ConfigureAwait(false);

                // Interactor Pipe
                await _InteractorPipeHandle.Invoke(inputPort, outputPort, serviceFactory, cancellationToken).ConfigureAwait(false);
            };

            s_InvokeHandleCache[_InputPortType] = _Value;
        }

        return ((Func<object, TOutputPort, ServiceFactory, CancellationToken, Task>)_Value)
            .Invoke(inputPort, outputPort, serviceFactory, cancellationToken);
    }

    private static Func<object, TOutputPort, ServiceFactory, CancellationToken, Task> GetFunc<TOutputPort>(Type inputPortType, MethodInfo methodInfo)
    {
        var _InputPort = Expression.Parameter(typeof(object));
        var _OutputPort = Expression.Parameter(typeof(TOutputPort));
        var _ServiceFactory = Expression.Parameter(typeof(ServiceFactory));
        var _CancellationToken = Expression.Parameter(typeof(CancellationToken));

        return Expression
            .Lambda<Func<object, TOutputPort, ServiceFactory, CancellationToken, Task>>(
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
