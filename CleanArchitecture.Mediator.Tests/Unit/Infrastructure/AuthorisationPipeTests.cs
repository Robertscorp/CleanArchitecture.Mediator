using CleanArchitecture.Mediator.Infrastructure;
using CleanArchitecture.Mediator.Pipeline;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Unit.Infrastructure
{

    public class AuthorisationPipeTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IAuthorisationResult> m_MockAuthorisationResult = new();
        private readonly Mock<IUseCaseAuthorisationEnforcer<TestInputPort, IAuthorisationResult>> m_MockEnforcer = new();
        private readonly Mock<UseCasePipeHandleAsync> m_MockNextHandleDelegate = new();
        private readonly Mock<IAuthorisationOutputPort<IAuthorisationResult>> m_MockOutputPort = new();
        private readonly Mock<UseCaseServiceResolver> m_MockServiceResolver = new();

        private readonly TestInputPort m_InputPort = new();
        private readonly IUseCasePipe m_Pipe;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public AuthorisationPipeTests()
        {
            this.m_Pipe = new AuthorisationPipe<IAuthorisationResult>(this.m_MockServiceResolver.Object);

            _ = this.m_MockServiceResolver
                    .Setup(mock => mock.Invoke(typeof(IUseCaseAuthorisationEnforcer<TestInputPort, IAuthorisationResult>)))
                    .Returns(this.m_MockEnforcer.Object);

            _ = this.m_MockEnforcer
                    .Setup(mock => mock.CheckAuthorisationAsync(this.m_InputPort, default))
                    .Returns(Task.FromResult(this.m_MockAuthorisationResult.Object));
        }

        #endregion Constructors

        #region - - - - - - HandleAsync Tests - - - - - -

        [Fact]
        public async Task HandleAsync_InputPortDoesNotSupportAuthorisation_InvokesNextHandleDelegate()
        {
            // Arrange
            var _InputPort = new object();

            // Act
            await this.m_Pipe.HandleAsync(_InputPort, this.m_MockOutputPort.Object, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Once());
        }

        [Fact]
        public async Task HandleAsync_OutputPortDoesNotSupportAuthorisation_InvokesNextHandleDelegate()
        {
            // Arrange
            var _OutputPort = new object();

            // Act
            await this.m_Pipe.HandleAsync(this.m_InputPort, _OutputPort, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Once());
        }

        [Fact]
        public async Task HandleAsync_EnforcerHasNotBeenRegistered_InvokesNextHandleDelegate()
        {
            // Arrange
            this.m_MockServiceResolver.Reset();

            // Act
            await this.m_Pipe.HandleAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Once());
            this.m_MockOutputPort.Verify(mock => mock.PresentUnauthorisedAsync(It.IsAny<IAuthorisationResult>(), default), Times.Never());
        }

        [Fact]
        public async Task HandleAsync_AuthorisationSuccessful_InvokesNextHandleDelegate()
        {
            // Arrange
            _ = this.m_MockAuthorisationResult
                    .Setup(mock => mock.IsAuthorised)
                    .Returns(true);

            // Act
            await this.m_Pipe.HandleAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Once());
            this.m_MockOutputPort.Verify(mock => mock.PresentUnauthorisedAsync(It.IsAny<IAuthorisationResult>(), default), Times.Never());
        }

        [Fact]
        public async Task HandleAsync_AuthorisationFails_PresentsUnauthorisedAsync()
        {
            // Arrange

            // Act
            await this.m_Pipe.HandleAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Never());
            this.m_MockOutputPort.Verify(mock => mock.PresentUnauthorisedAsync(It.IsAny<IAuthorisationResult>(), default), Times.Once());
        }

        #endregion HandleAsync Tests

        #region - - - - - - Nested Classes - - - - - -

        public class TestInputPort : IUseCaseInputPort<IAuthorisationOutputPort<IAuthorisationResult>> { }

        #endregion Nested Classes

    }

}
