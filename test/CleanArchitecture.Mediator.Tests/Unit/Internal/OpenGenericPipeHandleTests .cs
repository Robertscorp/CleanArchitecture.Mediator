using CleanArchitecture.Mediator.Internal;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Unit.Internal
{

    public class OpenGenericPipeHandleTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IPipeHandle> m_MockNextPipeHandle = new();
        private readonly Mock<IPipe<IInputPort<object>, object>> m_MockPipe = new();

        private readonly IInputPort<object> m_InputPort = new Mock<IInputPort<object>>().Object;
        private readonly object m_OutputPort = new();
        private readonly IPipeHandle m_PipeHandle;
        private readonly ServiceFactory m_ServiceFactory;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public OpenGenericPipeHandleTests()
        {
            this.m_PipeHandle = new OpenGenericPipeHandle(typeof(IPipe<,>), (inputPort, outputPort) => new Type[] { inputPort, outputPort }, this.m_MockNextPipeHandle.Object);

            var _MockServiceFactory = new Mock<ServiceFactory>();
            _ = _MockServiceFactory
                    .Setup(mock => mock(typeof(IPipe<IInputPort<object>, object>)))
                    .Returns(this.m_MockPipe.Object);

            this.m_ServiceFactory = _MockServiceFactory.Object;
        }

        #endregion Constructors

        #region - - - - - - InvokePipeAsync Tests - - - - - -

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

        [Fact]
        public async Task InvokePipeAsync_PipeDoesNotImplementGenericIPipeInterface_MovesToNextPipe()
        {
            // Arrange
            var _PipeHandle = (IPipeHandle)new OpenGenericPipeHandle(typeof(List<>), (inputPort, outputPort) => new Type[] { typeof(object) }, this.m_MockNextPipeHandle.Object);

            // Act
            await _PipeHandle.InvokePipeAsync(this.m_InputPort, this.m_OutputPort, this.m_ServiceFactory, default);

            // Assert
            this.m_MockNextPipeHandle.Verify(mock => mock.InvokePipeAsync(this.m_InputPort, this.m_OutputPort, this.m_ServiceFactory, default), Times.Once());

            this.m_MockPipe.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task InvokePipeAsync_PipeImplementsGenericIPipeInterfaceAndOperationIsNotCancelled_InvokesPipe()
        {
            // Arrange

            // Act
            await this.m_PipeHandle.InvokePipeAsync(this.m_InputPort, this.m_OutputPort, this.m_ServiceFactory, default);

            // Assert
            this.m_MockPipe.Verify(mock => mock.InvokeAsync(this.m_InputPort, this.m_OutputPort, this.m_ServiceFactory, this.m_MockNextPipeHandle.Object, default), Times.Once());

            this.m_MockPipe.VerifyNoOtherCalls();
        }

        #endregion InvokePipeAsync Tests

    }

}
