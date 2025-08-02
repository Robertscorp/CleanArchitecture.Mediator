using CleanArchitecture.Mediator.Internal;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Unit.Internal;

public class LicencePipeTests
{

    #region - - - - - - Fields - - - - - -

    private readonly Mock<ILicenceVerifier<IInputPort<ILicenceEnforcementOutputPort<object>>, object>> m_MockLicenceVerifier = new();
    private readonly Mock<NextPipeHandleAsync> m_MockNextPipeHandle = new();
    private readonly Mock<ILicenceEnforcementOutputPort<object>> m_MockOutputPort = new();
    private readonly Mock<ServiceFactory> m_MockServiceFactory = new();

    private readonly IInputPort<ILicenceEnforcementOutputPort<object>> m_InputPort = new Mock<IInputPort<ILicenceEnforcementOutputPort<object>>>().Object;
    private readonly IPipe<IInputPort<ILicenceEnforcementOutputPort<object>>, ILicenceEnforcementOutputPort<object>> m_Pipe
        = new LicencePipe<IInputPort<ILicenceEnforcementOutputPort<object>>, ILicenceEnforcementOutputPort<object>, object>();

    private bool m_IsLicenced = true;
    private object? m_LicenceFailure;

    #endregion Fields

    #region - - - - - - Constructors - - - - - -

    public LicencePipeTests()
    {
        var _LicenceFailure = new object();

        _ = this.m_MockLicenceVerifier
                .Setup(mock => mock.IsLicencedAsync(this.m_InputPort, out _LicenceFailure, this.m_MockServiceFactory.Object, default))
                .Returns(() =>
                {
                    this.m_LicenceFailure = _LicenceFailure;
                    return Task.FromResult(this.m_IsLicenced);
                });

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(ILicenceVerifier<IInputPort<ILicenceEnforcementOutputPort<object>>, object>)))
                .Returns(this.m_MockLicenceVerifier.Object);
    }

    #endregion Constructors

    #region - - - - - - InvokeAsync Tests - - - - - -

    [Fact]
    public async Task InvokeAsync_VerifierHasNotBeenRegistered_MovesToNextPipe()
    {
        // Arrange
        this.m_MockServiceFactory.Reset();

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockNextPipeHandle.Verify(mock => mock.Invoke(), Times.Once());

        this.m_MockLicenceVerifier.VerifyNoOtherCalls();
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
        this.m_MockLicenceVerifier.Verify(mock => mock.IsLicencedAsync(this.m_InputPort, out this.m_LicenceFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockNextPipeHandle.Verify(mock => mock.Invoke(), Times.Once());

        this.m_MockLicenceVerifier.VerifyNoOtherCalls();
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
        this.m_MockLicenceVerifier.Verify(mock => mock.IsLicencedAsync(this.m_InputPort, out this.m_LicenceFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockOutputPort.Verify(mock => mock.PresentLicenceFailureAsync(this.m_LicenceFailure!, default));

        this.m_MockLicenceVerifier.VerifyNoOtherCalls();
        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
    }

    #endregion InvokeAsync Tests

}
