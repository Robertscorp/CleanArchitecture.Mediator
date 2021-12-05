using CleanArchitecture.Services.Pipeline.Authentication;
using CleanArchitecture.Services.Pipeline.Infrastructure;
using FluentAssertions;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Services.Pipeline.Tests.Unit.Infrastructure
{

    public class AuthenticationUseCaseElementTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IAuthenticatedClaimsPrincipalProvider> m_MockClaimsPrincipalProvider = new();
        private readonly Mock<IServiceProvider> m_MockServiceProvdider = new();
        private readonly Mock<IAuthenticationOutputPort> m_MockOutputPort = new();

        private readonly AuthenticationUseCaseElement m_Element;
        private readonly object m_InputPort = new();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public AuthenticationUseCaseElementTests()
        {
            this.m_Element = new(this.m_MockServiceProvdider.Object);

            _ = this.m_MockServiceProvdider
                    .Setup(mock => mock.GetService(typeof(IAuthenticatedClaimsPrincipalProvider)))
                    .Returns(this.m_MockClaimsPrincipalProvider.Object);

            _ = this.m_MockClaimsPrincipalProvider
                    .Setup(mock => mock.AuthenticatedClaimsPrincipal)
                    .Returns(new ClaimsPrincipal());
        }

        #endregion Constructors

        #region - - - - - - TryOutputResultAsync Tests - - - - - -

        [Fact]
        public async Task TryOutputResultAsync_OutputPortDoesNotSupportAuthentication_ReturnsFalse()
        {
            // Arrange
            var _OutputPort = new object();

            // Act
            var _Actual = await this.m_Element.TryOutputResultAsync(this.m_InputPort, _OutputPort, default);

            // Assert
            _ = _Actual.Should().BeFalse();
        }

        [Fact]
        public async Task TryOutputResultAsync_ClaimsPrincipalProviderHasNotBeenRegistered_PresentsAuthenticationFailureAsyncAndReturnsTrue()
        {
            // Arrange
            this.m_MockServiceProvdider.Reset();

            // Act
            var _Actual = await this.m_Element.TryOutputResultAsync(this.m_InputPort, this.m_MockOutputPort.Object, default);

            // Assert
            _ = _Actual.Should().BeTrue();

            this.m_MockOutputPort.Verify(mock => mock.PresentUnauthenticatedAsync(default), Times.Once());
        }

        [Fact]
        public async Task TryOutputResultAsync_ClaimsPrincipalIsAuthenticated_ReturnsFalse()
        {
            // Arrange

            // Act
            var _Actual = await this.m_Element.TryOutputResultAsync(this.m_InputPort, this.m_MockOutputPort.Object, default);

            // Assert
            _ = _Actual.Should().BeFalse();

            this.m_MockOutputPort.Verify(mock => mock.PresentUnauthenticatedAsync(default), Times.Never());
        }

        [Fact]
        public async Task TryOutputResultAsync_NoAuthenticatedClaimsPrincipal_PresentsAuthenticationFailureAsyncAndReturnsTrue()
        {
            // Arrange
            this.m_MockClaimsPrincipalProvider.Reset();

            // Act
            var _Actual = await this.m_Element.TryOutputResultAsync(this.m_InputPort, this.m_MockOutputPort.Object, default);

            // Assert
            _ = _Actual.Should().BeTrue();

            this.m_MockOutputPort.Verify(mock => mock.PresentUnauthenticatedAsync(default), Times.Once());
        }

        #endregion TryOutputResultAsync Tests

    }

}
