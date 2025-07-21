using CleanArchitecture.Mediator.Internal;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Unit.Internal
{

    public class AuthenticationPipeTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IAuthenticatedClaimsPrincipalProvider> m_MockClaimsPrincipalProvider = new();
        private readonly Mock<IPipeHandle> m_MockNextPipeHandle = new();
        private readonly Mock<IAuthenticationOutputPort> m_MockOutputPort = new();
        private readonly Mock<ServiceFactory> m_MockServiceFactory = new();

        private readonly IInputPort<IAuthenticationOutputPort> m_InputPort = new Mock<IInputPort<IAuthenticationOutputPort>>().Object;
        private readonly IPipe m_Pipe = new AuthenticationPipe();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public AuthenticationPipeTests()
        {
            _ = this.m_MockServiceFactory
                    .Setup(mock => mock.Invoke(typeof(IAuthenticatedClaimsPrincipalProvider)))
                    .Returns(this.m_MockClaimsPrincipalProvider.Object);

            _ = this.m_MockClaimsPrincipalProvider
                    .Setup(mock => mock.AuthenticatedClaimsPrincipal)
                    .Returns(new ClaimsPrincipal());
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
            this.m_MockNextPipeHandle.Verify(mock => mock.InvokePipeAsync(this.m_InputPort, _OutputPort, this.m_MockServiceFactory.Object, default), Times.Once());

            this.m_MockClaimsPrincipalProvider.VerifyNoOtherCalls();
            this.m_MockNextPipeHandle.VerifyNoOtherCalls();
            this.m_MockOutputPort.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task InvokeAsync_ClaimsPrincipalProviderHasNotBeenRegistered_StopsWithAuthenticationFailure()
        {
            // Arrange
            this.m_MockServiceFactory.Reset();

            // Act
            await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

            // Assert
            this.m_MockOutputPort.Verify(mock => mock.PresentUnauthenticatedAsync(default), Times.Once());

            this.m_MockClaimsPrincipalProvider.VerifyNoOtherCalls();
            this.m_MockNextPipeHandle.VerifyNoOtherCalls();
            this.m_MockOutputPort.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task InvokeAsync_NoAuthenticatedClaimsPrincipal_StopsWithAuthenticationFailure()
        {
            // Arrange
            this.m_MockClaimsPrincipalProvider.Reset();

            // Act
            await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

            // Assert
            this.m_MockClaimsPrincipalProvider.Verify(mock => mock.AuthenticatedClaimsPrincipal);
            this.m_MockOutputPort.Verify(mock => mock.PresentUnauthenticatedAsync(default), Times.Once());

            this.m_MockClaimsPrincipalProvider.VerifyNoOtherCalls();
            this.m_MockNextPipeHandle.VerifyNoOtherCalls();
            this.m_MockOutputPort.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task InvokeAsync_ClaimsPrincipalIsAuthenticated_MovesToNextPipe()
        {
            // Arrange

            // Act
            await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

            // Assert
            this.m_MockClaimsPrincipalProvider.Verify(mock => mock.AuthenticatedClaimsPrincipal);
            this.m_MockNextPipeHandle.Verify(mock => mock.InvokePipeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Once());

            this.m_MockClaimsPrincipalProvider.VerifyNoOtherCalls();
            this.m_MockNextPipeHandle.VerifyNoOtherCalls();
            this.m_MockOutputPort.VerifyNoOtherCalls();
        }

        #endregion InvokeAsync Tests

    }

}
