using CleanArchitecture.Services.Pipeline.Authorisation;
using CleanArchitecture.Services.Pipeline.Infrastructure;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Services.Pipeline.Tests.Unit.Infrastructure
{

    public class AuthorisationUseCaseElementTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IUseCaseAuthorisationEnforcer<object, TestAuthorisationResult>> m_MockEnforcer = new();
        private readonly Mock<IAuthorisationOutputPort<TestAuthorisationResult>> m_MockOutputPort = new();
        private readonly Mock<IServiceProvider> m_MockServiceProvdider = new();

        private readonly TestAuthorisationResult m_AuthorisationResult = new();
        private readonly AuthorisationUseCaseElement<TestAuthorisationResult> m_Element;
        private readonly object m_InputPort = new();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public AuthorisationUseCaseElementTests()
        {
            this.m_Element = new(this.m_MockServiceProvdider.Object);

            _ = this.m_MockServiceProvdider
                    .Setup(mock => mock.GetService(typeof(IUseCaseAuthorisationEnforcer<object, TestAuthorisationResult>)))
                    .Returns(this.m_MockEnforcer.Object);

            _ = this.m_MockEnforcer
                    .Setup(mock => mock.CheckAuthorisationAsync(this.m_InputPort, default))
                    .Returns(Task.FromResult(this.m_AuthorisationResult));
        }

        #endregion Constructors

        #region - - - - - - TryOutputResultAsync Tests - - - - - -

        [Fact]
        public async Task TryOutputResultAsync_OutputPortDoesNotSupportAuthorisation_ReturnsFalse()
        {
            // Arrange
            var _OutputPort = new object();

            // Act
            var _Actual = await this.m_Element.TryOutputResultAsync(this.m_InputPort, _OutputPort, default);

            // Assert
            _ = _Actual.Should().BeFalse();
        }

        [Fact]
        public async Task TryOutputResultAsync_EnforcerHasNotBeenRegistered_ReturnsFalse()
        {
            // Arrange
            this.m_MockServiceProvdider.Reset();

            // Act
            var _Actual = await this.m_Element.TryOutputResultAsync(this.m_InputPort, this.m_MockOutputPort.Object, default);

            // Assert
            _ = _Actual.Should().BeFalse();

            this.m_MockOutputPort.Verify(mock => mock.PresentUnauthorisedAsync(It.IsAny<TestAuthorisationResult>(), default), Times.Never());
        }

        [Fact]
        public async Task TryOutputResultAsync_AuthorisationSuccessful_ReturnsFalse()
        {
            // Arrange
            this.m_AuthorisationResult.IsAuthorised = true;

            // Act
            var _Actual = await this.m_Element.TryOutputResultAsync(this.m_InputPort, this.m_MockOutputPort.Object, default);

            // Assert
            _ = _Actual.Should().BeFalse();

            this.m_MockOutputPort.Verify(mock => mock.PresentUnauthorisedAsync(It.IsAny<TestAuthorisationResult>(), default), Times.Never());
        }

        [Fact]
        public async Task TryOutputResultAsync_AuthorisationFails_PresentsUnauthorisedAsyncAndReturnsTrue()
        {
            // Arrange

            // Act
            var _Actual = await this.m_Element.TryOutputResultAsync(this.m_InputPort, this.m_MockOutputPort.Object, default);

            // Assert
            _ = _Actual.Should().BeTrue();

            this.m_MockOutputPort.Verify(mock => mock.PresentUnauthorisedAsync(It.IsAny<TestAuthorisationResult>(), default), Times.Once());
        }

        #endregion TryOutputResultAsync Tests

        #region - - - - - - Nested Classes - - - - - -

        public class TestAuthorisationResult : IAuthorisationResult
        {

            #region - - - - - - IAuthorisationResult Implementation - - - - - -

            public bool IsAuthorised { get; set; }

            #endregion IAuthorisationResult Implementation

        }

        #endregion Nested Classes

    }

}
