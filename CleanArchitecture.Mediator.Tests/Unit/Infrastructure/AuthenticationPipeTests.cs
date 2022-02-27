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
        private readonly Mock<UseCasePipeHandleAsync> m_MockNextHandleDelegate = new();
        private readonly Mock<UseCaseServiceResolver> m_MockServiceResolver = new();
        private readonly Mock<IAuthenticationOutputPort> m_MockOutputPort = new();

        private readonly object m_InputPort = new();
        private readonly IUseCasePipe m_Pipe;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public AuthenticationPipeTests()
        {
            this.m_Pipe = new AuthenticationPipe(this.m_MockServiceResolver.Object);

            _ = this.m_MockServiceResolver
                    .Setup(mock => mock.Invoke(typeof(IAuthenticatedClaimsPrincipalProvider)))
                    .Returns(this.m_MockClaimsPrincipalProvider.Object);

            _ = this.m_MockClaimsPrincipalProvider
                    .Setup(mock => mock.AuthenticatedClaimsPrincipal)
                    .Returns(new ClaimsPrincipal());
        }

        #endregion Constructors

        #region - - - - - - HandleAsync Tests - - - - - -

        [Fact]
        public async Task HandleAsync_OutputPortDoesNotSupportAuthentication_InvokesNextHandleDelegate()
        {
            // Arrange
            var _OutputPort = new object();

            // Act
            await this.m_Pipe.HandleAsync(this.m_InputPort, _OutputPort, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Once());
        }

        [Fact]
        public async Task HandleAsync_ClaimsPrincipalProviderHasNotBeenRegistered_PresentsAuthenticationFailureAsync()
        {
            // Arrange
            this.m_MockServiceResolver.Reset();

            // Act
            await this.m_Pipe.HandleAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Never());
            this.m_MockOutputPort.Verify(mock => mock.PresentUnauthenticatedAsync(default), Times.Once());
        }

        [Fact]
        public async Task HandleAsync_ClaimsPrincipalIsAuthenticated_InvokesNextHandleDelegate()
        {
            // Arrange

            // Act
            await this.m_Pipe.HandleAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Once());
            this.m_MockOutputPort.Verify(mock => mock.PresentUnauthenticatedAsync(default), Times.Never());
        }

        [Fact]
        public async Task HandleAsync_NoAuthenticatedClaimsPrincipal_PresentsAuthenticationFailureAsync()
        {
            // Arrange
            this.m_MockClaimsPrincipalProvider.Reset();

            // Act
            await this.m_Pipe.HandleAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Never());
            this.m_MockOutputPort.Verify(mock => mock.PresentUnauthenticatedAsync(default), Times.Once());
        }

        #endregion HandleAsync Tests

    }

}
