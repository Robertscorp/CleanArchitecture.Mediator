using CleanArchitecture.Services.Infrastructure;
using CleanArchitecture.Services.Pipeline;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Services.Tests.Unit.Infrastructure
{

    public class AuthorisationUseCaseElementTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IUseCaseAuthorisationEnforcer<TestInputPort, TestAuthorisationResult>> m_MockEnforcer = new();
        private readonly Mock<UseCaseElementHandleAsync> m_MockNextHandleDelegate = new();
        private readonly Mock<IAuthorisationOutputPort<TestAuthorisationResult>> m_MockOutputPort = new();
        private readonly Mock<UseCaseServiceResolver> m_MockServiceResolver = new();

        private readonly TestAuthorisationResult m_AuthorisationResult = new();
        private readonly IUseCaseElement m_Element;
        private readonly TestInputPort m_InputPort = new();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public AuthorisationUseCaseElementTests()
        {
            this.m_Element = new AuthorisationUseCaseElement<TestAuthorisationResult>(this.m_MockServiceResolver.Object);

            _ = this.m_MockServiceResolver
                    .Setup(mock => mock.Invoke(typeof(IUseCaseAuthorisationEnforcer<TestInputPort, TestAuthorisationResult>)))
                    .Returns(this.m_MockEnforcer.Object);

            _ = this.m_MockEnforcer
                    .Setup(mock => mock.CheckAuthorisationAsync(this.m_InputPort, default))
                    .Returns(Task.FromResult(this.m_AuthorisationResult));
        }

        #endregion Constructors

        #region - - - - - - HandleAsync Tests - - - - - -

        [Fact]
        public async Task HandleAsync_InputPortDoesNotSupportAuthorisation_InvokesNextHandleDelegate()
        {
            // Arrange
            var _InputPort = new object();

            // Act
            await this.m_Element.HandleAsync(_InputPort, this.m_MockOutputPort.Object, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Once());
        }

        [Fact]
        public async Task HandleAsync_OutputPortDoesNotSupportAuthorisation_InvokesNextHandleDelegate()
        {
            // Arrange
            var _OutputPort = new object();

            // Act
            await this.m_Element.HandleAsync(this.m_InputPort, _OutputPort, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Once());
        }

        [Fact]
        public async Task HandleAsync_EnforcerHasNotBeenRegistered_InvokesNextHandleDelegate()
        {
            // Arrange
            this.m_MockServiceResolver.Reset();

            // Act
            await this.m_Element.HandleAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Once());
            this.m_MockOutputPort.Verify(mock => mock.PresentUnauthorisedAsync(It.IsAny<TestAuthorisationResult>(), default), Times.Never());
        }

        [Fact]
        public async Task HandleAsync_AuthorisationSuccessful_InvokesNextHandleDelegate()
        {
            // Arrange
            this.m_AuthorisationResult.IsAuthorised = true;

            // Act
            await this.m_Element.HandleAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Once());
            this.m_MockOutputPort.Verify(mock => mock.PresentUnauthorisedAsync(It.IsAny<TestAuthorisationResult>(), default), Times.Never());
        }

        [Fact]
        public async Task HandleAsync_AuthorisationFails_PresentsUnauthorisedAsync()
        {
            // Arrange

            // Act
            await this.m_Element.HandleAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Never());
            this.m_MockOutputPort.Verify(mock => mock.PresentUnauthorisedAsync(It.IsAny<TestAuthorisationResult>(), default), Times.Once());
        }

        #endregion HandleAsync Tests

        #region - - - - - - Nested Classes - - - - - -

        public class TestAuthorisationResult : IAuthorisationResult
        {

            #region - - - - - - IAuthorisationResult Implementation - - - - - -

            public bool IsAuthorised { get; set; }

            #endregion IAuthorisationResult Implementation

        }

        public class TestInputPort : IUseCaseInputPort<IAuthorisationOutputPort<TestAuthorisationResult>> { }

        #endregion Nested Classes

    }

}
