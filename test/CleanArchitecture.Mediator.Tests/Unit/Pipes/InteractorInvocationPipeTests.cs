using CleanArchitecture.Mediator.Pipes;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Unit.Pipes
{

    public class InteractorInvocationPipeTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IInteractor<IInputPort<object>, object>> m_MockInteractor = new();
        private readonly Mock<IPipe> m_MockPipe = new();
        private readonly Mock<ServiceFactory> m_MockServiceFactory = new();

        private readonly IInputPort<object> m_InputPort = new Mock<IInputPort<object>>().Object;
        private readonly object m_OutputPort = new();
        private readonly IPipe m_Pipe;
        private readonly PipeHandle m_PipeHandle;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public InteractorInvocationPipeTests()
        {
            this.m_Pipe = new InteractorInvocationPipe();
            this.m_PipeHandle = new(this.m_MockPipe.Object, null);

            _ = this.m_MockServiceFactory
                    .Setup(mock => mock.Invoke(typeof(IInteractor<IInputPort<object>, object>)))
                    .Returns(this.m_MockInteractor.Object);
        }

        #endregion Constructors

        #region - - - - - - InvokeAsync Tests - - - - - -

        [Fact]
        public async Task InvokeAsync_InteractorDoesNotExist_DoesNothing()
        {
            // Arrange
            this.m_MockServiceFactory.Reset();

            // Act
            var _Exception = await Record.ExceptionAsync(() => this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_OutputPort, this.m_MockServiceFactory.Object, this.m_PipeHandle, default));

            // Assert
            _ = _Exception.Should().BeNull();

            this.m_MockPipe.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task InvokeAsync_InteractorExists_InvokesInteractor()
        {
            // Arrange

            // Act
            await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_OutputPort, this.m_MockServiceFactory.Object, this.m_PipeHandle, default);

            // Assert
            this.m_MockInteractor.Verify(mock => mock.HandleAsync(this.m_InputPort, this.m_OutputPort, default), Times.Once());
            this.m_MockPipe.VerifyNoOtherCalls();
        }

        #endregion InvokeAsync Tests

    }

}
