using CleanArchitecture.Mediator.Sample.Application.Services.Validation;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace CleanArchitecture.Mediator.Sample;

internal partial class TestPipeline
{

    private static readonly MethodInfo s_InvokeAuthenticationPipeAsyncMethodInfo = typeof(TestPipeline).GetMethod(nameof(InvokeAuthenticationPipeAsync), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo s_InvokeAuthorisationPipeAsyncMethodInfo = typeof(TestPipeline).GetMethod(nameof(InvokeAuthorisationPipeAsync), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo s_InvokeInputPortValidationPipeAsyncMethodInfo = typeof(TestPipeline).GetMethod(nameof(InvokeInputPortValidationPipeAsync), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo s_InvokeInteractorInvocationPipeAsyncMethodInfo = typeof(TestPipeline).GetMethod(nameof(InvokeInteractorInvocationPipeAsync), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo s_InvokeValidationPipeAsyncMethodInfo = typeof(TestPipeline).GetMethod(nameof(InvokeValidationPipeAsync), BindingFlags.NonPublic | BindingFlags.Static)!;

    private readonly ConcurrentDictionary<Type, Func<object, object, ServiceFactory, CancellationToken, Task>> m_AuthenticationPipeHandleCache = [];
    private readonly ConcurrentDictionary<Type, Func<object, object, ServiceFactory, CancellationToken, Task>> m_AuthorisationPipeHandleCache = [];
    private readonly ConcurrentDictionary<Type, Func<object, object, ServiceFactory, CancellationToken, Task>> m_InputPortValidationPipeHandleCache = [];
    private readonly ConcurrentDictionary<Type, Func<object, object, ServiceFactory, CancellationToken, Task>> m_InteractorInvocationPipeHandleCache = [];
    private readonly ConcurrentDictionary<Type, Func<object, object, ServiceFactory, CancellationToken, Task>> m_ValidationPipeHandleCache = [];

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
    public async Task InvokeAsync<TOutputPort>(
        IInputPort<TOutputPort> inputPort,
        TOutputPort outputPort,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
    {
        if (inputPort == null) throw new ArgumentNullException(nameof(inputPort));
        if (outputPort == null) throw new ArgumentNullException(nameof(outputPort));

        var _InputPortType = inputPort.GetType();
        var _OutputPortType = typeof(TOutputPort);

        // Authentication Pipe
        await this.m_AuthenticationPipeHandleCache
            .GetOrAdd(_InputPortType, key => GetFunc(_InputPortType, _OutputPortType, s_InvokeAuthenticationPipeAsyncMethodInfo))
            .Invoke(inputPort, outputPort, serviceFactory, cancellationToken)
            .ConfigureAwait(false);

        // Authorisation Pipe
        if (inputPort is IInputPort<IInputPortValidationFailureOutputPort<SampleInputPortValidationFailure>> &&
            outputPort is IInputPortValidationFailureOutputPort<SampleInputPortValidationFailure>)
            await this.m_InputPortValidationPipeHandleCache
                .GetOrAdd(
                    _InputPortType,
                    key => GetFunc(_InputPortType, _OutputPortType, s_InvokeAuthorisationPipeAsyncMethodInfo))
                .Invoke(inputPort, outputPort, serviceFactory, cancellationToken)
                .ConfigureAwait(false);

        // Input Port Validation Pipe
        if (inputPort is IInputPort<IInputPortValidationFailureOutputPort<SampleInputPortValidationFailure>> &&
            outputPort is IInputPortValidationFailureOutputPort<SampleInputPortValidationFailure>)
            await this.m_InputPortValidationPipeHandleCache
                .GetOrAdd(
                    _InputPortType,
                    key => GetFunc(_InputPortType, _OutputPortType, s_InvokeInputPortValidationPipeAsyncMethodInfo))
                .Invoke(inputPort, outputPort, serviceFactory, cancellationToken)
                .ConfigureAwait(false);

        // Validation Pipe
        await this.m_ValidationPipeHandleCache
            .GetOrAdd(_InputPortType, key => GetFunc(_InputPortType, _OutputPortType, s_InvokeValidationPipeAsyncMethodInfo))
            .Invoke(inputPort, outputPort, serviceFactory, cancellationToken)
            .ConfigureAwait(false);

        // Interactor Pipe
        await this.m_InteractorInvocationPipeHandleCache
            .GetOrAdd(_InputPortType, key => GetFunc(_InputPortType, _OutputPortType, s_InvokeInteractorInvocationPipeAsyncMethodInfo))
            .Invoke(inputPort, outputPort, serviceFactory, cancellationToken)
            .ConfigureAwait(false);
    }

    private static Func<object, object, ServiceFactory, CancellationToken, Task> GetFunc(Type inputPortType, Type outputPortType, MethodInfo methodInfo)
    {
        var _InputPort = Expression.Parameter(typeof(object));
        var _OutputPort = Expression.Parameter(typeof(object));
        var _ServiceFactory = Expression.Parameter(typeof(ServiceFactory));
        var _CancellationToken = Expression.Parameter(typeof(CancellationToken));

        return Expression
            .Lambda<Func<object, object, ServiceFactory, CancellationToken, Task>>(
                Expression.Call(
                    null,
                    methodInfo.MakeGenericMethod(inputPortType, outputPortType),
                    [Expression.Convert(_InputPort, inputPortType), Expression.Convert(_OutputPort, outputPortType), _ServiceFactory, _CancellationToken]),
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
        _ = serviceFactory
            .GetService<IInteractor<TInputPort, TOutputPort>>()
            .HandleAsync(inputPort, outputPort, serviceFactory, cancellationToken);

        return ContinuationBehaviour.ContinueAsync;
    }

}
