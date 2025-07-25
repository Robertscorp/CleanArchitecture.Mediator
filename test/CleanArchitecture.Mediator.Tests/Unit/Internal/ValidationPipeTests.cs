﻿using CleanArchitecture.Mediator.Internal;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Unit.Internal;

public class ValidationPipeTests
{

    #region - - - - - - Fields - - - - - -

    private readonly Mock<NextPipeHandleAsync> m_MockNextPipeHandle = new();
    private readonly Mock<ITestOutputPort> m_MockOutputPort = new();
    private readonly Mock<ServiceFactory> m_MockServiceFactory = new();
    private readonly Mock<IValidator<TestInputPort, ITestOutputPort>> m_MockValidator = new();

    private readonly TestInputPort m_InputPort = new();
    private readonly IPipe m_Pipe = new ValidationPipe();

    private bool m_ValidationResult;

    #endregion Fields

    #region - - - - - - Constructors - - - - - -

    public ValidationPipeTests()
    {
        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(IValidator<TestInputPort, ITestOutputPort>)))
                .Returns(this.m_MockValidator.Object);

        _ = this.m_MockValidator
                .Setup(mock => mock.HandleValidationAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, default))
                .Returns(() => Task.FromResult(this.m_ValidationResult));
    }

    #endregion Constructors

    #region - - - - - - InvokeAsync Tests - - - - - -

    [Fact]
    public async Task InvokeAsync_OutputPortDoesNotSupportValidation_MovesToNextPipe()
    {
        // Arrange
        var _OutputPort = new object();

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, _OutputPort, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockNextPipeHandle.Verify(mock => mock.Invoke(), Times.Once());

        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
        this.m_MockValidator.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_ValidatorHasNotBeenRegistered_MovesToNextPipe()
    {
        // Arrange
        this.m_MockServiceFactory.Reset();

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockNextPipeHandle.Verify(mock => mock.Invoke(), Times.Once());

        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
        this.m_MockValidator.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_ValidationSuccessful_MovesToNextPipe()
    {
        // Arrange
        this.m_ValidationResult = true;

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockNextPipeHandle.Verify(mock => mock.Invoke(), Times.Once());
        this.m_MockValidator.Verify(mock => mock.HandleValidationAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Once());

        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
        this.m_MockValidator.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_ValidationFails_StopsWithValidationFailure()
    {
        // Arrange

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockValidator.Verify(mock => mock.HandleValidationAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Once());

        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
        this.m_MockValidator.VerifyNoOtherCalls();
    }

    #endregion InvokeAsync Tests

    #region - - - - - - Nested Classes - - - - - -

    public class TestInputPort : IInputPort<ITestOutputPort> { }

    public interface ITestOutputPort { }

    #endregion Nested Classes

}
