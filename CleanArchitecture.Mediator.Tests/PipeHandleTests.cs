using FluentAssertions;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests
{

    public class PipeHandleTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly IInputPort<object> m_InputPort = new Mock<IInputPort<object>>().Object;
        private readonly PipeHandle m_NextPipeHandle = new(null, null);
        private readonly object m_OutputPort = new();
        private readonly IPipe m_Pipe = new Mock<IPipe>().Object;

        #endregion Fields

        #region - - - - - - InvokePipeAsync Tests - - - - - -

        [Fact]
        public async Task InvokePipeAsync_NotCancelled_RunsToCompletion()
        {
            // Arrange
            var _PipeHandle = new PipeHandle(this.m_Pipe, this.m_NextPipeHandle);

            // Act
            var _Actual = _PipeHandle.InvokePipeAsync(this.m_InputPort, this.m_OutputPort, default, default);
            await _Actual;

            // Assert
            _ = _Actual.Status.Should().Be(TaskStatus.RanToCompletion);
        }

        [Fact]
        public async Task InvokePipeAsync_Cancelled_DoesNotRunToCompletion()
        {
            // Arrange
            var _PipeHandle = new PipeHandle(this.m_Pipe, this.m_NextPipeHandle);
            var _CancellationTokenSource = new CancellationTokenSource();

            // Act
            _CancellationTokenSource.Cancel();

            var _Actual = _PipeHandle.InvokePipeAsync(this.m_InputPort, this.m_OutputPort, default, _CancellationTokenSource.Token);
            var _Exception = await Record.ExceptionAsync(() => _Actual);

            // Assert
            _ = _Actual.Status.Should().Be(TaskStatus.Canceled);
            _ = _Exception.Should().BeOfType<TaskCanceledException>();
        }

        [Fact]
        public void InvokePipeAsync_PipeIsNull_ReturnsCompletedTask()
        {
            // Arrange
            var _PipeHandle = new PipeHandle(default, this.m_NextPipeHandle);

            // Act
            var _Actual = _PipeHandle.InvokePipeAsync(this.m_InputPort, this.m_OutputPort, default, default);

            // Assert
            _ = _Actual.Should().Be(Task.CompletedTask);
        }

        #endregion InvokePipeAsync Tests

    }

}
