using CleanArchitecture.Mediator.Internal;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Unit.Internal;

public class LicencePolicyValidationPipeTests
{

    #region - - - - - - Fields - - - - - -

    private readonly Mock<ILicencePolicyValidator<IInputPort<ILicencePolicyFailureOutputPort<object>>, object>> m_MockLicencePolicyValidator = new();
    private readonly Mock<NextPipeHandleAsync> m_MockNextPipeHandle = new();
    private readonly Mock<ILicencePolicyFailureOutputPort<object>> m_MockOutputPort = new();
    private readonly Mock<ServiceFactory> m_MockServiceFactory = new();

    private readonly IInputPort<ILicencePolicyFailureOutputPort<object>> m_InputPort = new Mock<IInputPort<ILicencePolicyFailureOutputPort<object>>>().Object;
    private readonly IPipe<IInputPort<ILicencePolicyFailureOutputPort<object>>, ILicencePolicyFailureOutputPort<object>> m_Pipe
        = new LicencePolicyValidationPipe<IInputPort<ILicencePolicyFailureOutputPort<object>>, ILicencePolicyFailureOutputPort<object>, object>();

    private bool m_IsLicenced = true;
    private object? m_LicencePolicyFailure;

    #endregion Fields

    #region - - - - - - Constructors - - - - - -

    public LicencePolicyValidationPipeTests()
    {
        var _PolicyFailure = new object();

        _ = this.m_MockLicencePolicyValidator
                .Setup(mock => mock.ValidateAsync(this.m_InputPort, out _PolicyFailure, this.m_MockServiceFactory.Object, default))
                .Returns(() =>
                {
                    this.m_LicencePolicyFailure = _PolicyFailure;
                    return Task.FromResult(this.m_IsLicenced);
                });

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(ILicencePolicyValidator<IInputPort<ILicencePolicyFailureOutputPort<object>>, object>)))
                .Returns(this.m_MockLicencePolicyValidator.Object);
    }

    #endregion Constructors

    #region - - - - - - InvokeAsync Tests - - - - - -

    [Fact]
    public async Task InvokeAsync_ValidatorHasNotBeenRegistered_InvocationFails()
    {
        // Arrange
        this.m_MockServiceFactory.Reset();

        // Act
        var _Exception = await Record.ExceptionAsync(() => this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default));

        // Assert
        _ = _Exception.Should().BeOfType<NullReferenceException>();

        this.m_MockLicencePolicyValidator.VerifyNoOtherCalls();
        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_PrincipalIsLicenced_MovesToNextPipe()
    {
        // Arrange

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockLicencePolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_LicencePolicyFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockNextPipeHandle.Verify(mock => mock.Invoke(), Times.Once());

        this.m_MockLicencePolicyValidator.VerifyNoOtherCalls();
        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_PrincipalIsNotLicenced_StopsWithLicenceFailure()
    {
        // Arrange
        this.m_IsLicenced = false;

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockLicencePolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_LicencePolicyFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockOutputPort.Verify(mock => mock.PresentLicencePolicyFailureAsync(this.m_LicencePolicyFailure!, default));

        this.m_MockLicencePolicyValidator.VerifyNoOtherCalls();
        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
    }

    #endregion InvokeAsync Tests

}
