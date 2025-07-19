using CleanArchitecture.Mediator.Internal;
using FluentAssertions;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Unit.Internal
{

    public class NonGenericPipeHandleTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IPipe> m_MockPipe = new();

        private readonly IInputPort<object> m_InputPort = new Mock<IInputPort<object>>().Object;
        private readonly object m_OutputPort = new();
        private readonly IPipeHandle m_NextPipeHandle = new Mock<IPipeHandle>().Object;
        private readonly IPipeHandle m_PipeHandle;
        private readonly ServiceFactory m_ServiceFactory = new Mock<ServiceFactory>().Object;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public NonGenericPipeHandleTests()
            => this.m_PipeHandle = new NonGenericPipeHandle(this.m_MockPipe.Object, this.m_NextPipeHandle);

        #endregion Constructors

        #region - - - - - - InvokePipeAsync Tests - - - - - -

        [Fact]
        public async Task InvokePipeAsync_OperationIsNotCancelled_InvokesPipe()
        {
            // Arrange

            // Act
            await this.m_PipeHandle.InvokePipeAsync(this.m_InputPort, this.m_OutputPort, this.m_ServiceFactory, default);

            // Assert
            this.m_MockPipe.Verify(mock => mock.InvokeAsync(this.m_InputPort, this.m_OutputPort, this.m_ServiceFactory, this.m_NextPipeHandle, default), Times.Once());

            this.m_MockPipe.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task InvokePipeAsync_OperationIsCancelled_CancelsImmediately()
        {
            // Arrange
            var _CancellationTokenSource = new CancellationTokenSource();

            // Act
            _CancellationTokenSource.Cancel();

            var _Actual = this.m_PipeHandle.InvokePipeAsync(this.m_InputPort, this.m_OutputPort, default, _CancellationTokenSource.Token);
            var _Exception = await Record.ExceptionAsync(() => _Actual);

            // Assert
            _ = _Actual.Status.Should().Be(TaskStatus.Canceled);
            _ = _Exception.Should().BeOfType<TaskCanceledException>();

            this.m_MockPipe.VerifyNoOtherCalls();
        }

        #endregion InvokePipeAsync Tests

    }

}
