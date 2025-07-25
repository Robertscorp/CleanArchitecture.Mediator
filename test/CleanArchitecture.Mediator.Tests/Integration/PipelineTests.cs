﻿using CleanArchitecture.Mediator.Internal;
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

    private readonly Mock<IAuthorisationEnforcer<InputPort, IEverythingOutputPort>> m_MockAuthEnforcer = new();
    private readonly Mock<IAuthenticatedClaimsPrincipalProvider> m_MockClaimsPrincipalProvider = new();
    private readonly Mock<IEmptyOutputPort> m_MockEmptyOutputPort = new();
    private readonly Mock<IInteractor<InputPort, IEmptyOutputPort>> m_MockEmptyOutputPortInteractor = new();
    private readonly Mock<IEverythingOutputPort> m_MockEverythingOutputPort = new();
    private readonly Mock<IInputPortValidator<InputPort, object>> m_MockInputPortValidator = new();
    private readonly Mock<ServiceFactory> m_MockServiceFactory = new();
    private readonly Mock<IValidator<InputPort, IEverythingOutputPort>> m_MockValidator = new();

    private readonly InputPort m_InputPort = new();
    private readonly Pipeline m_Pipeline;

    private bool m_AuthResult = true;
    private object? m_InputPortValidationFailure;
    private bool m_IsDataValid = true;
    private bool m_IsInputPortValid = true;

    #endregion Fields

    #region - - - - - - Constructors - - - - - -

    public PipelineTests()
    {
        var _InputPortValidationFailure = new object();

        _ = this.m_MockAuthEnforcer
                .Setup(mock => mock.HandleAuthorisationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default))
                .Returns(() => Task.FromResult(this.m_AuthResult));

        _ = this.m_MockClaimsPrincipalProvider
                .Setup(mock => mock.AuthenticatedClaimsPrincipal)
                .Returns(new ClaimsPrincipal());

        _ = this.m_MockInputPortValidator
                .Setup(mock => mock.ValidateAsync(this.m_InputPort, out _InputPortValidationFailure, It.IsAny<ServiceFactory>(), default))
                .Returns(() =>
                {
                    this.m_InputPortValidationFailure = _InputPortValidationFailure;
                    return Task.FromResult(this.m_IsInputPortValid);
                });

        _ = this.m_MockValidator
                .Setup(mock => mock.HandleValidationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default))
                .Returns(() => Task.FromResult(this.m_IsDataValid));

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(IAuthorisationEnforcer<InputPort, IEverythingOutputPort>)))
                .Returns(this.m_MockAuthEnforcer.Object);

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(IAuthenticatedClaimsPrincipalProvider)))
                .Returns(this.m_MockClaimsPrincipalProvider.Object);

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(IInputPortValidator<InputPort, object>)))
                .Returns(this.m_MockInputPortValidator.Object);

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(IInteractor<InputPort, IEmptyOutputPort>)))
                .Returns(this.m_MockEmptyOutputPortInteractor.Object);

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(IValidator<InputPort, IEverythingOutputPort>)))
                .Returns(this.m_MockValidator.Object);

        CleanArchitectureMediator.Setup(
            config =>
                config.AddPipeline<Pipeline>(
                    pipeline =>
                        pipeline
                            .AddAuthentication()
                            .AddAuthorisation()
                            .AddInputPortValidation<object>()
                            .AddValidation()
                            .AddInteractorInvocation()),
            registration =>
                registration
                    .WithSingletonFactoryRegistrationAction((type, factory) => this.m_MockServiceFactory.Setup(mock => mock.Invoke(type)).Returns(factory(this.m_MockServiceFactory.Object)))
                    .WithSingletonServiceRegistrationAction((type, implementationType) => this.m_MockServiceFactory.Setup(mock => mock.Invoke(type)).Returns((Type t) => Activator.CreateInstance(implementationType)!)));

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(InputPortValidationPipe<InputPort, IEverythingOutputPort, object>)))
                .Returns(() => new InputPortValidationPipe<InputPort, IEverythingOutputPort, object>());

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
        this.m_MockClaimsPrincipalProvider.Reset();

        // Act
        await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default);

        // Assert
        this.m_MockAuthEnforcer.Verify(mock => mock.HandleAuthorisationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Never());
        this.m_MockClaimsPrincipalProvider.Verify(mock => mock.AuthenticatedClaimsPrincipal, Times.Once());
        this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, this.m_MockServiceFactory.Object, default), Times.Never());
        this.m_MockValidator.Verify(mock => mock.HandleValidationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Never());

        this.m_MockEverythingOutputPort.Verify(mock => mock.PresentUnauthenticatedAsync(default), Times.Once());
        this.m_MockEverythingOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_EverythingOutputPortWithAuthorisationFailure_StopsWithAuthorisationFailure()
    {
        // Arrange
        this.m_AuthResult = false;

        // Act
        await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default);

        // Assert
        this.m_MockAuthEnforcer.Verify(mock => mock.HandleAuthorisationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockClaimsPrincipalProvider.Verify(mock => mock.AuthenticatedClaimsPrincipal, Times.Once());
        this.m_MockValidator.Verify(mock => mock.HandleValidationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Never());

        this.m_MockEverythingOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_EverythingOutputPortWithInputPortValidationFailure_StopsWithInputPortValidationFailure()
    {
        // Arrange
        this.m_IsInputPortValid = false;

        // Act
        await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default);

        // Assert
        this.m_MockAuthEnforcer.Verify(mock => mock.HandleAuthorisationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockClaimsPrincipalProvider.Verify(mock => mock.AuthenticatedClaimsPrincipal, Times.Once());
        this.m_MockEverythingOutputPort.Verify(mock => mock.PresentInputPortValidationFailureAsync(this.m_InputPortValidationFailure!, default), Times.Once());
        this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockValidator.Verify(mock => mock.HandleValidationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Never());

        this.m_MockEverythingOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_EverythingOutputPortWithValidationFailure_StopsWithValidationFailure()
    {
        // Arrange
        this.m_IsDataValid = false;

        // Act
        await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default);

        // Assert
        this.m_MockAuthEnforcer.Verify(mock => mock.HandleAuthorisationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockClaimsPrincipalProvider.Verify(mock => mock.AuthenticatedClaimsPrincipal, Times.Once());
        this.m_MockValidator.Verify(mock => mock.HandleValidationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Once());

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
        this.m_MockClaimsPrincipalProvider.Reset();

        // Act
        await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, services => { }, default);

        // Assert
        this.m_MockAuthEnforcer.Verify(mock => mock.HandleAuthorisationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default), Times.Never());
        this.m_MockClaimsPrincipalProvider.Verify(mock => mock.AuthenticatedClaimsPrincipal, Times.Once());
        this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, It.IsAny<ServiceFactory>(), default), Times.Never());
        this.m_MockValidator.Verify(mock => mock.HandleValidationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default), Times.Never());

        this.m_MockEverythingOutputPort.Verify(mock => mock.PresentUnauthenticatedAsync(default), Times.Once());
        this.m_MockEverythingOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_WithInvocationServiceCollection_EverythingOutputPortWithAuthorisationFailure_StopsWithAuthorisationFailure()
    {
        // Arrange
        this.m_AuthResult = false;

        // Act
        await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, services => { }, default);

        // Assert
        this.m_MockAuthEnforcer.Verify(mock => mock.HandleAuthorisationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default), Times.Once());
        this.m_MockClaimsPrincipalProvider.Verify(mock => mock.AuthenticatedClaimsPrincipal, Times.Once());
        this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, It.IsAny<ServiceFactory>(), default), Times.Never());
        this.m_MockValidator.Verify(mock => mock.HandleValidationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default), Times.Never());

        this.m_MockEverythingOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_WithInvocationServiceCollection_EverythingOutputPortWithInputPortValidationFailure_StopsWithInputPortValidationFailure()
    {
        // Arrange
        this.m_IsInputPortValid = false;

        // Act
        await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, services => { }, default);

        // Assert
        this.m_MockAuthEnforcer.Verify(mock => mock.HandleAuthorisationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default), Times.Once());
        this.m_MockClaimsPrincipalProvider.Verify(mock => mock.AuthenticatedClaimsPrincipal, Times.Once());
        this.m_MockEverythingOutputPort.Verify(mock => mock.PresentInputPortValidationFailureAsync(this.m_InputPortValidationFailure!, default), Times.Once());
        this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, It.IsAny<ServiceFactory>(), default), Times.Once());
        this.m_MockValidator.Verify(mock => mock.HandleValidationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default), Times.Never());

        this.m_MockEverythingOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_WithInvocationServiceCollection_EverythingOutputPortWithValidationFailure_StopsWithValidationFailure()
    {
        // Arrange
        this.m_IsDataValid = false;

        // Act
        await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, services => { }, default);

        // Assert
        this.m_MockAuthEnforcer.Verify(mock => mock.HandleAuthorisationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default), Times.Once());
        this.m_MockClaimsPrincipalProvider.Verify(mock => mock.AuthenticatedClaimsPrincipal, Times.Once());
        this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_InputPortValidationFailure, It.IsAny<ServiceFactory>(), default), Times.Once());
        this.m_MockValidator.Verify(mock => mock.HandleValidationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, It.IsAny<ServiceFactory>(), default), Times.Once());

        this.m_MockEverythingOutputPort.VerifyNoOtherCalls();
    }

    #endregion InvokeAsync (w/ InvocationServiceCollection) Tests

    #region - - - - - - Nested Classes - - - - - -

    public interface IEmptyOutputPort { }

    public interface IEverythingOutputPort : IAuthenticationOutputPort, IInputPortValidationOutputPort<object> { }

    public class InputPort : IInputPort<IEmptyOutputPort>, IInputPort<IEverythingOutputPort> { }

    #endregion Nested Classes

}
