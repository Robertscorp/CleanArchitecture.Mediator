using CleanArchitecture.Mediator.Authentication;
using CleanArchitecture.Mediator.Infrastructure;
using CleanArchitecture.Mediator.Pipeline;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Unit.Infrastructure
{

    public class AuthenticationPipeTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IAuthenticatedClaimsPrincipalProvider> m_MockClaimsPrincipalProvider = new();
        private readonly Mock<IAuthenticationOutputPort> m_MockOutputPort = new();
        private readonly Mock<IPipe> m_MockPipe = new();
        private readonly Mock<ServiceFactory> m_MockServiceFactory = new();

        private readonly object m_InputPort = new();
        private readonly PipeHandle m_NextPipeHandle = new(null, null);
        private readonly IPipe m_Pipe;
        private readonly PipeHandle m_PipeHandle;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public AuthenticationPipeTests()
        {
            this.m_Pipe = new AuthenticationPipe();
            this.m_PipeHandle = new(this.m_MockPipe.Object, this.m_NextPipeHandle);

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
            await this.m_Pipe.InvokeAsync(this.m_InputPort, _OutputPort, this.m_MockServiceFactory.Object, this.m_PipeHandle, default);

            // Assert
            this.m_MockPipe.Verify(mock => mock.InvokeAsync(this.m_InputPort, _OutputPort, this.m_MockServiceFactory.Object, this.m_NextPipeHandle, default), Times.Once());
        }

        [Fact]
        public async Task InvokeAsync_ClaimsPrincipalProviderHasNotBeenRegistered_StopsWithAuthenticationFailure()
        {
            // Arrange
            this.m_MockServiceFactory.Reset();

            // Act
            await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_PipeHandle, default);

            // Assert
            this.m_MockOutputPort.Verify(mock => mock.PresentUnauthenticatedAsync(default), Times.Once());
            this.m_MockPipe.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task InvokeAsync_NoAuthenticatedClaimsPrincipal_StopsWithAuthenticationFailure()
        {
            // Arrange
            this.m_MockClaimsPrincipalProvider.Reset();

            // Act
            await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_PipeHandle, default);

            // Assert
            this.m_MockOutputPort.Verify(mock => mock.PresentUnauthenticatedAsync(default), Times.Once());
            this.m_MockPipe.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task InvokeAsync_ClaimsPrincipalIsAuthenticated_MovesToNextPipe()
        {
            // Arrange

            // Act
            await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_PipeHandle, default);

            // Assert
            this.m_MockOutputPort.Verify(mock => mock.PresentUnauthenticatedAsync(default), Times.Never());
            this.m_MockPipe.Verify(mock => mock.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_NextPipeHandle, default), Times.Once());
        }

        #endregion InvokeAsync Tests

    }

}
