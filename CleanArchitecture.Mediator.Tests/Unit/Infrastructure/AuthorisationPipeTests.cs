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
        private readonly Mock<IAuthorisationOutputPort<IAuthorisationResult>> m_MockOutputPort = new();
        private readonly Mock<IPipe> m_MockPipe = new();
        private readonly Mock<ServiceFactory> m_MockServiceFactory = new();

        private readonly TestInputPort m_InputPort = new();
        private readonly PipeHandle m_NextPipeHandle = new(null, null);
        private readonly IPipe m_Pipe;
        private readonly PipeHandle m_PipeHandle;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public AuthorisationPipeTests()
        {
            this.m_Pipe = new AuthorisationPipe<IAuthorisationResult>();
            this.m_PipeHandle = new(this.m_MockPipe.Object, this.m_NextPipeHandle);

            _ = this.m_MockEnforcer
                    .Setup(mock => mock.CheckAuthorisationAsync(this.m_InputPort, default))
                    .Returns(Task.FromResult(this.m_MockAuthorisationResult.Object));

            _ = this.m_MockServiceFactory
                    .Setup(mock => mock.Invoke(typeof(IUseCaseAuthorisationEnforcer<TestInputPort, IAuthorisationResult>)))
                    .Returns(this.m_MockEnforcer.Object);
        }

        #endregion Constructors

        #region - - - - - - InvokeAsync Tests - - - - - -

        [Fact]
        public async Task InvokeAsync_InputPortDoesNotSupportAuthorisation_MovesToNextPipe()
        {
            // Arrange
            var _InputPort = new object();

            // Act
            await this.m_Pipe.InvokeAsync(_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_PipeHandle, default);

            // Assert
            this.m_MockPipe.Verify(mock => mock.InvokeAsync(_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_NextPipeHandle, default), Times.Once());
            this.m_MockOutputPort.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task InvokeAsync_OutputPortDoesNotSupportAuthorisation_MovesToNextPipe()
        {
            // Arrange
            var _OutputPort = new object();

            // Act
            await this.m_Pipe.InvokeAsync(this.m_InputPort, _OutputPort, this.m_MockServiceFactory.Object, this.m_PipeHandle, default);

            // Assert
            this.m_MockPipe.Verify(mock => mock.InvokeAsync(this.m_InputPort, _OutputPort, this.m_MockServiceFactory.Object, this.m_NextPipeHandle, default), Times.Once());
        }

        [Fact]
        public async Task InvokeAsync_EnforcerHasNotBeenRegistered_MovesToNextPipe()
        {
            // Arrange
            this.m_MockServiceFactory.Reset();

            // Act
            await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_PipeHandle, default);

            // Assert
            this.m_MockPipe.Verify(mock => mock.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_NextPipeHandle, default), Times.Once());
            this.m_MockOutputPort.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task InvokeAsync_AuthorisationSuccessful_MovesToNextPipe()
        {
            // Arrange
            _ = this.m_MockAuthorisationResult
                    .Setup(mock => mock.IsAuthorised)
                    .Returns(true);

            // Act
            await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_PipeHandle, default);

            // Assert
            this.m_MockPipe.Verify(mock => mock.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_NextPipeHandle, default), Times.Once());
            this.m_MockOutputPort.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task InvokeAsync_AuthorisationFails_StopsWithAuthorisationFailure()
        {
            // Arrange

            // Act
            await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_PipeHandle, default);

            // Assert
            this.m_MockOutputPort.Verify(mock => mock.PresentUnauthorisedAsync(It.IsAny<IAuthorisationResult>(), default), Times.Once());
            this.m_MockPipe.VerifyNoOtherCalls();
        }

        #endregion InvokeAsync Tests

        #region - - - - - - Nested Classes - - - - - -

        public class TestInputPort : IUseCaseInputPort<IAuthorisationOutputPort<IAuthorisationResult>> { }

        #endregion Nested Classes

    }

}
