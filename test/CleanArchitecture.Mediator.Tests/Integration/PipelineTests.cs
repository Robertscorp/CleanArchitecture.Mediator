using CleanArchitecture.Mediator.Internal;
using CleanArchitecture.Mediator.Setup;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Integration;

public class PipelineTests
{

    #region - - - - - - Fields - - - - - -

    private readonly Mock<IAuthorisationPolicyValidator<InputPort, object>> m_MockAuthorisationPolicyValidator = new();
    private readonly Mock<IBusinessRuleEvaluator<InputPort, IEverythingOutputPort>> m_MockBusinessRuleEvaluator = new();
    private readonly Mock<ContinuationBehaviour> m_MockContinuationBehaviour = new(10);
    private readonly Mock<IEmptyOutputPort> m_MockEmptyOutputPort = new();
    private readonly Mock<IInteractor<InputPort, IEmptyOutputPort>> m_MockEmptyOutputPortInteractor = new();
    private readonly Mock<IEverythingOutputPort> m_MockEverythingOutputPort = new();
    private readonly Mock<IInteractor<InputPort, IEverythingOutputPort>> m_MockEverythingOutputPortInteractor = new();
    private readonly Mock<IInputPortValidator<InputPort, object>> m_MockInputPortValidator = new();
    private readonly Mock<ILicencePolicyValidator<InputPort, object>> m_MockLicencePolicyValidator = new();
    private readonly Mock<IPrincipalAccessor> m_MockPrincipalAccessor = new();
    private readonly Mock<ServiceFactory> m_MockServiceFactory = new();

    private readonly InputPort m_InputPort = new();
    private readonly Pipeline m_Pipeline;

    private object? m_AuthorisationPolicyFailure;
    private ContinuationBehaviour m_BusinessRuleContinuation = ContinuationBehaviour.Continue;
    private object? m_InputPortValidationFailure;
    private bool m_IsAuthorised = true;
    private bool m_IsInputPortValid = true;
    private bool m_IsLicenced = true;
    private object? m_LicencePolicyFailure;

    #endregion Fields

    #region - - - - - - Constructors - - - - - -

    public PipelineTests()
    {
        var _AuthorisationPolicyFailure = new object();
        var _InputPortValidationFailure = new object();
        var _LicencePolicyFailure = new object();

        _ = this.m_MockAuthorisationPolicyValidator
                .Setup(mock => mock.ValidateAsync(this.m_InputPort, out _AuthorisationPolicyFailure, It.IsAny<ServiceFactory>(), default))
                .Returns(() =>
                {
                    this.m_AuthorisationPolicyFailure = _AuthorisationPolicyFailure;
                    return Task.FromResult(this.m_IsAuthorised);
                });

        _ = this.m_MockBusinessRuleEvaluator
                .Setup(mock => mock.EvaluateAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default))
                .Returns(() => Task.FromResult(this.m_BusinessRuleContinuation));

        _ = this.m_MockContinuationBehaviour
                .Setup(mock => mock.HandleAsync(It.IsAny<NextPipeHandleAsync>(), default))
                .Returns(Task.CompletedTask);

        _ = this.m_MockEverythingOutputPort
                .Setup(mock => mock.PresentAuthorisationPolicyFailureAsync(_AuthorisationPolicyFailure, default))
                .Returns(Task.FromResult(this.m_MockContinuationBehaviour.Object));

        _ = this.m_MockEverythingOutputPort
                .Setup(mock => mock.PresentInputPortValidationFailureAsync(_InputPortValidationFailure, default))
                .Returns(Task.FromResult(this.m_MockContinuationBehaviour.Object));

        _ = this.m_MockEverythingOutputPort
                .Setup(mock => mock.PresentLicencePolicyFailureAsync(_LicencePolicyFailure, default))
                .Returns(Task.FromResult(this.m_MockContinuationBehaviour.Object));

        _ = this.m_MockInputPortValidator
                .Setup(mock => mock.ValidateAsync(this.m_InputPort, out _InputPortValidationFailure, It.IsAny<ServiceFactory>(), default))
                .Returns(() =>
                {
                    this.m_InputPortValidationFailure = _InputPortValidationFailure;
                    return Task.FromResult(this.m_IsInputPortValid);
                });

        _ = this.m_MockLicencePolicyValidator
                .Setup(mock => mock.ValidateAsync(this.m_InputPort, out _LicencePolicyFailure, It.IsAny<ServiceFactory>(), default))
                .Returns(() =>
                {
                    this.m_LicencePolicyFailure = _LicencePolicyFailure;
                    return Task.FromResult(this.m_IsLicenced);
                });

        _ = this.m_MockPrincipalAccessor
                .Setup(mock => mock.Principal)
                .Returns(new ClaimsPrincipal());

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(IAuthorisationPolicyValidator<InputPort, object>)))
                .Returns(this.m_MockAuthorisationPolicyValidator.Object);

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(IBusinessRuleEvaluator<InputPort, IEverythingOutputPort>)))
                .Returns(this.m_MockBusinessRuleEvaluator.Object);

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(IInputPortValidator<InputPort, object>)))
                .Returns(this.m_MockInputPortValidator.Object);

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(IInteractor<InputPort, IEmptyOutputPort>)))
                .Returns(this.m_MockEmptyOutputPortInteractor.Object);

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(IInteractor<InputPort, IEverythingOutputPort>)))
                .Returns(this.m_MockEverythingOutputPortInteractor.Object);

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(ILicencePolicyValidator<InputPort, object>)))
                .Returns(this.m_MockLicencePolicyValidator.Object);

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(IPrincipalAccessor)))
                .Returns(this.m_MockPrincipalAccessor.Object);

        CleanArchitectureMediator.Setup(
            config =>
                config.AddPipeline<Pipeline>(
                    pipeline =>
                        pipeline
                            .AddAuthentication(AuthenticationMode.SinglePrincipal)
                            .AddAuthorisationPolicyValidation<object>()
                            .AddLicencePolicyValidation<object>()
                            .AddInputPortValidation<object>()
                            .AddBusinessRuleEvaluation()
                            .AddInteractorInvocation()),
            registration =>
                registration
                    .WithSingletonFactoryRegistrationAction((type, factory) => this.m_MockServiceFactory.Setup(mock => mock.Invoke(type)).Returns(factory(this.m_MockServiceFactory.Object)))
                    .WithSingletonServiceRegistrationAction((type, implementationType) => this.m_MockServiceFactory.Setup(mock => mock.Invoke(type)).Returns((Type t) => Activator.CreateInstance(implementationType)!)));

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(AuthorisationPolicyValidationPipe<InputPort, IEverythingOutputPort, object>)))
                .Returns(() => new AuthorisationPolicyValidationPipe<InputPort, IEverythingOutputPort, object>());

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(InputPortValidationPipe<InputPort, IEverythingOutputPort, object>)))
                .Returns(() => new InputPortValidationPipe<InputPort, IEverythingOutputPort, object>());

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(LicencePolicyValidationPipe<InputPort, IEverythingOutputPort, object>)))
                .Returns(() => new LicencePolicyValidationPipe<InputPort, IEverythingOutputPort, object>());

        this.m_Pipeline = new Pipeline(this.m_MockServiceFactory.Object);
    }

    #endregion Constructors

    #region - - - - - - InvokeAsync Tests - - - - - -

    [Fact]
    public async Task InvokeAsync_EmptyOutputPort_InvokesInteractor()
    {
        // Arrange

        // Act
        await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEmptyOutputPort.Object, this.m_MockServiceFactory.Object, default);

        // Assert
        this.m_MockEmptyOutputPortInteractor.Verify(mock => mock.HandleAsync(this.m_InputPort, this.m_MockEmptyOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Once());
    }

    [Fact]
    public async Task InvokeAsync_EverythingOutputPortWithAuthenticationFailure_StopsWithAuthenticationFailure()
    {
        // Arrange
        this.m_MockPrincipalAccessor.Reset();

        // Act
        await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default);

        // Assert
        this.m_MockAuthorisationPolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_AuthorisationPolicyFailure, this.m_MockServiceFactory.Object, default), Times.Never());
        this.m_MockBusinessRuleEvaluator.Verify(mock => mock.EvaluateAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Never());
        this.m_MockContinuationBehaviour.Verify(mock => mock.HandleAsync(It.IsAny<NextPipeHandleAsync>(), default), Times.Never());
        this.m_MockEverythingOutputPort.Verify(mock => mock.PresentAuthenticationFailureAsync(default), Times.Once());
        this.m_MockEverythingOutputPortInteractor.Verify(mock => mock.HandleAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Never());
        this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, this.m_MockServiceFactory.Object, default), Times.Never());
        this.m_MockLicencePolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, this.m_MockServiceFactory.Object, default), Times.Never());
        this.m_MockPrincipalAccessor.Verify(mock => mock.Principal, Times.Once());

        this.m_MockEverythingOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_EverythingOutputPortWithAuthorisationPolicyFailure_InvokesContinuationBehaviourFromOutputPort()
    {
        // Arrange
        this.m_IsAuthorised = false;

        // Act
        await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default);

        // Assert
        this.m_MockAuthorisationPolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_AuthorisationPolicyFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockBusinessRuleEvaluator.Verify(mock => mock.EvaluateAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Never());
        this.m_MockContinuationBehaviour.Verify(mock => mock.HandleAsync(It.IsAny<NextPipeHandleAsync>(), default), Times.Once());
        this.m_MockEverythingOutputPort.Verify(mock => mock.PresentAuthorisationPolicyFailureAsync(this.m_AuthorisationPolicyFailure!, default), Times.Once());
        this.m_MockEverythingOutputPortInteractor.Verify(mock => mock.HandleAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Never());
        this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, this.m_MockServiceFactory.Object, default), Times.Never());
        this.m_MockLicencePolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, this.m_MockServiceFactory.Object, default), Times.Never());
        this.m_MockPrincipalAccessor.Verify(mock => mock.Principal, Times.Once());

        this.m_MockEverythingOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_EverythingOutputPortWithLicencePolicyFailure_InvokesContinuationBehaviourFromOutputPort()
    {
        // Arrange
        this.m_IsLicenced = false;

        // Act
        await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default);

        // Assert
        this.m_MockAuthorisationPolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_AuthorisationPolicyFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockBusinessRuleEvaluator.Verify(mock => mock.EvaluateAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Never());
        this.m_MockContinuationBehaviour.Verify(mock => mock.HandleAsync(It.IsAny<NextPipeHandleAsync>(), default), Times.Once());
        this.m_MockEverythingOutputPort.Verify(mock => mock.PresentLicencePolicyFailureAsync(this.m_LicencePolicyFailure!, default), Times.Once());
        this.m_MockEverythingOutputPortInteractor.Verify(mock => mock.HandleAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Never());
        this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, this.m_MockServiceFactory.Object, default), Times.Never());
        this.m_MockLicencePolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockPrincipalAccessor.Verify(mock => mock.Principal, Times.Once());

        this.m_MockEverythingOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_EverythingOutputPortWithInputPortValidationFailure_InvokesContinuationBehaviourFromOutputPort()
    {
        // Arrange
        this.m_IsInputPortValid = false;

        // Act
        await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default);

        // Assert
        this.m_MockAuthorisationPolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_AuthorisationPolicyFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockBusinessRuleEvaluator.Verify(mock => mock.EvaluateAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Never());
        this.m_MockContinuationBehaviour.Verify(mock => mock.HandleAsync(It.IsAny<NextPipeHandleAsync>(), default), Times.Once());
        this.m_MockEverythingOutputPort.Verify(mock => mock.PresentInputPortValidationFailureAsync(this.m_InputPortValidationFailure!, default), Times.Once());
        this.m_MockEverythingOutputPortInteractor.Verify(mock => mock.HandleAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Never());
        this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockLicencePolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockPrincipalAccessor.Verify(mock => mock.Principal, Times.Once());

        this.m_MockEverythingOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_EverythingOutputPortWithBusinessRuleEvaluationProvidingReturnBehaviour_InvokesReturnBehaviour()
    {
        // Arrange
        this.m_BusinessRuleContinuation = ContinuationBehaviour.Return;

        // Act
        await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default);

        // Assert
        this.m_MockAuthorisationPolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_AuthorisationPolicyFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockBusinessRuleEvaluator.Verify(mock => mock.EvaluateAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockContinuationBehaviour.Verify(mock => mock.HandleAsync(It.IsAny<NextPipeHandleAsync>(), default), Times.Never());
        this.m_MockEverythingOutputPortInteractor.Verify(mock => mock.HandleAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Never());
        this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockLicencePolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockPrincipalAccessor.Verify(mock => mock.Principal, Times.Once());

        this.m_MockEverythingOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_EverythingOutputPortWithSuccessfulPipeline_InvokesInteractor()
    {
        // Arrange

        // Act
        await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default);

        // Assert
        this.m_MockAuthorisationPolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_AuthorisationPolicyFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockBusinessRuleEvaluator.Verify(mock => mock.EvaluateAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockContinuationBehaviour.Verify(mock => mock.HandleAsync(It.IsAny<NextPipeHandleAsync>(), default), Times.Never());
        this.m_MockEverythingOutputPortInteractor.Verify(mock => mock.HandleAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockLicencePolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockPrincipalAccessor.Verify(mock => mock.Principal, Times.Once());

        this.m_MockEverythingOutputPort.VerifyNoOtherCalls();
    }

    #endregion InvokeAsync Tests

    #region - - - - - - InvokeAsync (w/ InvocationServiceCollection) Tests - - - - - -

    [Fact]
    public async Task InvokeAsync_WithInvocationServiceCollection_EmptyOutputPort_InvokesInteractor()
    {
        // Arrange

        // Act
        await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEmptyOutputPort.Object, this.m_MockServiceFactory.Object, services => { }, default);

        // Assert
        this.m_MockEmptyOutputPortInteractor.Verify(mock => mock.HandleAsync(this.m_InputPort, this.m_MockEmptyOutputPort.Object, It.IsAny<ServiceFactory>(), default), Times.Once());
    }

    [Fact]
    public async Task InvokeAsync_WithInvocationServiceCollection_OverridenService_UsesOverridenService()
    {
        // Arrange
        var _MockInteractor = new Mock<IInteractor<InputPort, IEmptyOutputPort>>();

        // Act
        await this.m_Pipeline.InvokeAsync(
            this.m_InputPort,
            this.m_MockEmptyOutputPort.Object,
            this.m_MockServiceFactory.Object,
            services => services.WithService<IInteractor<InputPort, IEmptyOutputPort>>(_MockInteractor.Object),
            default);

        // Assert
        _MockInteractor.Verify(mock => mock.HandleAsync(this.m_InputPort, this.m_MockEmptyOutputPort.Object, It.IsAny<ServiceFactory>(), default), Times.Once());
        this.m_MockEmptyOutputPortInteractor.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_WithInvocationServiceCollection_EverythingOutputPortWithAuthenticationFailure_StopsWithAuthenticationFailure()
    {
        // Arrange
        this.m_MockPrincipalAccessor.Reset();

        // Act
        await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, services => { }, default);

        // Assert
        this.m_MockAuthorisationPolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_AuthorisationPolicyFailure, It.IsAny<ServiceFactory>(), default), Times.Never());
        this.m_MockBusinessRuleEvaluator.Verify(mock => mock.EvaluateAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default), Times.Never());
        this.m_MockContinuationBehaviour.Verify(mock => mock.HandleAsync(It.IsAny<NextPipeHandleAsync>(), default), Times.Never());
        this.m_MockEverythingOutputPort.Verify(mock => mock.PresentAuthenticationFailureAsync(default), Times.Once());
        this.m_MockEverythingOutputPortInteractor.Verify(mock => mock.HandleAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default), Times.Never());
        this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, It.IsAny<ServiceFactory>(), default), Times.Never());
        this.m_MockLicencePolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_LicencePolicyFailure, It.IsAny<ServiceFactory>(), default), Times.Never());
        this.m_MockPrincipalAccessor.Verify(mock => mock.Principal, Times.Once());

        this.m_MockEverythingOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_WithInvocationServiceCollection_EverythingOutputPortWithAuthorisationPolicyFailure_InvokesContinuationBehaviourFromOutputPort()
    {
        // Arrange
        this.m_IsAuthorised = false;

        // Act
        await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, services => { }, default);

        // Assert
        this.m_MockAuthorisationPolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_AuthorisationPolicyFailure, It.IsAny<ServiceFactory>(), default), Times.Once());
        this.m_MockBusinessRuleEvaluator.Verify(mock => mock.EvaluateAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default), Times.Never());
        this.m_MockContinuationBehaviour.Verify(mock => mock.HandleAsync(It.IsAny<NextPipeHandleAsync>(), default), Times.Once());
        this.m_MockEverythingOutputPort.Verify(mock => mock.PresentAuthorisationPolicyFailureAsync(this.m_AuthorisationPolicyFailure!, default));
        this.m_MockEverythingOutputPortInteractor.Verify(mock => mock.HandleAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default), Times.Never());
        this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, It.IsAny<ServiceFactory>(), default), Times.Never());
        this.m_MockLicencePolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_LicencePolicyFailure, It.IsAny<ServiceFactory>(), default), Times.Never());
        this.m_MockPrincipalAccessor.Verify(mock => mock.Principal, Times.Once());

        this.m_MockEverythingOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_WithInvocationServiceCollection_EverythingOutputPortWithLicencePolicyFailure_InvokesContinuationBehaviourFromOutputPort()
    {
        // Arrange
        this.m_IsLicenced = false;

        // Act
        await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, services => { }, default);

        // Assert
        this.m_MockAuthorisationPolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_AuthorisationPolicyFailure, It.IsAny<ServiceFactory>(), default), Times.Once());
        this.m_MockBusinessRuleEvaluator.Verify(mock => mock.EvaluateAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default), Times.Never());
        this.m_MockContinuationBehaviour.Verify(mock => mock.HandleAsync(It.IsAny<NextPipeHandleAsync>(), default), Times.Once());
        this.m_MockEverythingOutputPort.Verify(mock => mock.PresentLicencePolicyFailureAsync(this.m_LicencePolicyFailure!, default));
        this.m_MockEverythingOutputPortInteractor.Verify(mock => mock.HandleAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default), Times.Never());
        this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, It.IsAny<ServiceFactory>(), default), Times.Never());
        this.m_MockLicencePolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_LicencePolicyFailure, It.IsAny<ServiceFactory>(), default), Times.Once());
        this.m_MockPrincipalAccessor.Verify(mock => mock.Principal, Times.Once());

        this.m_MockEverythingOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_WithInvocationServiceCollection_EverythingOutputPortWithInputPortValidationFailure_InvokesContinuationBehaviourFromOutputPort()
    {
        // Arrange
        this.m_IsInputPortValid = false;

        // Act
        await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, services => { }, default);

        // Assert
        this.m_MockAuthorisationPolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_AuthorisationPolicyFailure, It.IsAny<ServiceFactory>(), default), Times.Once());
        this.m_MockBusinessRuleEvaluator.Verify(mock => mock.EvaluateAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default), Times.Never());
        this.m_MockContinuationBehaviour.Verify(mock => mock.HandleAsync(It.IsAny<NextPipeHandleAsync>(), default), Times.Once());
        this.m_MockEverythingOutputPort.Verify(mock => mock.PresentInputPortValidationFailureAsync(this.m_InputPortValidationFailure!, default), Times.Once());
        this.m_MockEverythingOutputPortInteractor.Verify(mock => mock.HandleAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default), Times.Never());
        this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, It.IsAny<ServiceFactory>(), default), Times.Once());
        this.m_MockLicencePolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_LicencePolicyFailure, It.IsAny<ServiceFactory>(), default), Times.Once());
        this.m_MockPrincipalAccessor.Verify(mock => mock.Principal, Times.Once());

        this.m_MockEverythingOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_WithInvocationServiceCollection_EverythingOutputPortWithBusinessRuleEvaluationProvidingReturnBehaviour_InvokesReturnBehaviour()
    {
        // Arrange
        this.m_BusinessRuleContinuation = ContinuationBehaviour.Return;

        // Act
        await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, services => { }, default);

        // Assert
        this.m_MockAuthorisationPolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_AuthorisationPolicyFailure, It.IsAny<ServiceFactory>(), default), Times.Once());
        this.m_MockBusinessRuleEvaluator.Verify(mock => mock.EvaluateAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default), Times.Once());
        this.m_MockContinuationBehaviour.Verify(mock => mock.HandleAsync(It.IsAny<NextPipeHandleAsync>(), default), Times.Never());
        this.m_MockEverythingOutputPortInteractor.Verify(mock => mock.HandleAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default), Times.Never());
        this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, It.IsAny<ServiceFactory>(), default), Times.Once());
        this.m_MockLicencePolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_LicencePolicyFailure, It.IsAny<ServiceFactory>(), default), Times.Once());
        this.m_MockPrincipalAccessor.Verify(mock => mock.Principal, Times.Once());

        this.m_MockEverythingOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_WithInvocationServiceCollection_EverythingOutputPortWithSuccessfulPipeline_InvokesInteractor()
    {
        // Arrange

        // Act
        await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, services => { }, default);

        // Assert
        this.m_MockAuthorisationPolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_AuthorisationPolicyFailure, It.IsAny<ServiceFactory>(), default), Times.Once());
        this.m_MockBusinessRuleEvaluator.Verify(mock => mock.EvaluateAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default), Times.Once());
        this.m_MockContinuationBehaviour.Verify(mock => mock.HandleAsync(It.IsAny<NextPipeHandleAsync>(), default), Times.Never());
        this.m_MockEverythingOutputPortInteractor.Verify(mock => mock.HandleAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default), Times.Once());
        this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, It.IsAny<ServiceFactory>(), default), Times.Once());
        this.m_MockLicencePolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_LicencePolicyFailure, It.IsAny<ServiceFactory>(), default), Times.Once());
        this.m_MockPrincipalAccessor.Verify(mock => mock.Principal, Times.Once());

        this.m_MockEverythingOutputPort.VerifyNoOtherCalls();
    }

    #endregion InvokeAsync (w/ InvocationServiceCollection) Tests

    #region - - - - - - Nested Classes - - - - - -

    public interface IEmptyOutputPort { }

    public interface IEverythingOutputPort :
        IAuthenticationFailureOutputPort,
        IAuthorisationPolicyFailureOutputPort<object>,
        IInputPortValidationFailureOutputPort<object>,
        ILicencePolicyFailureOutputPort<object>
    { }

    public class InputPort : IInputPort<IEmptyOutputPort>, IInputPort<IEverythingOutputPort> { }

    #endregion Nested Classes

}
