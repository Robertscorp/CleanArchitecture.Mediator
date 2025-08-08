using CleanArchitecture.Mediator.Internal;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Unit.Internal;

public class AuthenticationPipeTests
{

    #region - - - - - - Fields - - - - - -

    private readonly Mock<NextPipeHandleAsync> m_MockNextPipeHandle = new();
    private readonly Mock<IAuthenticationFailureOutputPort> m_MockOutputPort = new();
    private readonly Mock<IPrincipalAccessor> m_MockPrincipalAccessor = new();
    private readonly Mock<ServiceFactory> m_MockServiceFactory = new();

    private readonly IInputPort<IAuthenticationFailureOutputPort> m_InputPort = new Mock<IInputPort<IAuthenticationFailureOutputPort>>().Object;
    private readonly IPipe m_Pipe = new AuthenticationPipe();

    #endregion Fields

    #region - - - - - - Constructors - - - - - -

    public AuthenticationPipeTests()
    {
        _ = this.m_MockPrincipalAccessor
                .Setup(mock => mock.Principal)
                .Returns(new ClaimsPrincipal());

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(IPrincipalAccessor)))
                .Returns(this.m_MockPrincipalAccessor.Object);
    }

    #endregion Constructors

    #region - - - - - - InvokeAsync Tests - - - - - -

    [Fact]
    public async Task InvokeAsync_OutputPortDoesNotSupportAuthentication_MovesToNextPipe()
    {
        // Arrange
        var _OutputPort = new object();

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, _OutputPort, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockNextPipeHandle.Verify(mock => mock.Invoke(), Times.Once());

        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
        this.m_MockPrincipalAccessor.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_PrincipalAccessorHasNotBeenRegistered_StopsWithAuthenticationFailure()
    {
        // Arrange
        this.m_MockServiceFactory.Reset();

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockOutputPort.Verify(mock => mock.PresentAuthenticationFailureAsync(default), Times.Once());

        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
        this.m_MockPrincipalAccessor.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_NoAuthenticatedPrincipal_StopsWithAuthenticationFailure()
    {
        // Arrange
        this.m_MockPrincipalAccessor.Reset();

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockOutputPort.Verify(mock => mock.PresentAuthenticationFailureAsync(default), Times.Once());
        this.m_MockPrincipalAccessor.Verify(mock => mock.Principal);

        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
        this.m_MockPrincipalAccessor.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_PrincipalIsAuthenticated_MovesToNextPipe()
    {
        // Arrange

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockNextPipeHandle.Verify(mock => mock.Invoke(), Times.Once());
        this.m_MockPrincipalAccessor.Verify(mock => mock.Principal);

        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
        this.m_MockPrincipalAccessor.VerifyNoOtherCalls();
    }

    #endregion InvokeAsync Tests

}
