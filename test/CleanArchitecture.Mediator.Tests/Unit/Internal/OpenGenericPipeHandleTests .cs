using CleanArchitecture.Mediator.Internal;
using FluentAssertions;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Unit.Internal;

public class OpenGenericPipeHandleTests
{

    #region - - - - - - Fields - - - - - -

    private readonly Mock<IPipeHandle> m_MockNextPipeHandle = new();
    private readonly Mock<IPipe<IInputPort<object>, object>> m_MockPipe = new();
    private readonly Mock<ServiceFactory> m_MockServiceFactory = new();

    private readonly IInputPort<object> m_InputPort = new Mock<IInputPort<object>>().Object;
    private readonly object m_OutputPort = new();
    private readonly IPipeHandle m_PipeHandle;

    #endregion Fields

    #region - - - - - - Constructors - - - - - -

    public OpenGenericPipeHandleTests()
    {
        _ = this.m_MockServiceFactory
                .Setup(mock => mock(typeof(IPipe<IInputPort<object>, object>)))
                .Returns(this.m_MockPipe.Object);

        this.m_PipeHandle = new OpenGenericPipeHandle(new PipeProvider(), this.m_MockNextPipeHandle.Object);
    }

    #endregion Constructors

    #region - - - - - - InvokePipeAsync Tests - - - - - -

    [Fact]
    public async Task InvokePipeAsync_InvokingNextPipeHandle_InvokesCorrectly()
    {
        // Arrange
        _ = this.m_MockPipe
                .Setup(mock => mock.InvokeAsync(this.m_InputPort, this.m_OutputPort, this.m_MockServiceFactory.Object, It.IsAny<NextPipeHandleAsync>(), default))
                .Returns((IInputPort<object> ip, object op, ServiceFactory sf, NextPipeHandleAsync nextPipeHandle, CancellationToken ct)
                    => nextPipeHandle());

        // Act
        await this.m_PipeHandle.InvokePipeAsync(this.m_InputPort, this.m_OutputPort, this.m_MockServiceFactory.Object, default);

        // Assert
        this.m_MockNextPipeHandle.Verify(mock => mock.InvokePipeAsync(this.m_InputPort, this.m_OutputPort, this.m_MockServiceFactory.Object, default), Times.Once());

        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
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

        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockPipe.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokePipeAsync_OperationIsNotCancelledAndNoPipeProvided_MovesToNextPipe()
    {
        // Arrange
        this.m_MockServiceFactory.Reset();

        // Act
        await this.m_PipeHandle.InvokePipeAsync(this.m_InputPort, this.m_OutputPort, this.m_MockServiceFactory.Object, default);

        // Assert
        this.m_MockNextPipeHandle.Verify(mock => mock.InvokePipeAsync(this.m_InputPort, this.m_OutputPort, this.m_MockServiceFactory.Object, default), Times.Once());

        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockPipe.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokePipeAsync_OperationIsNotCancelledAndPipeProvided_InvokesPipe()
    {
        // Arrange

        // Act
        await this.m_PipeHandle.InvokePipeAsync(this.m_InputPort, this.m_OutputPort, this.m_MockServiceFactory.Object, default);

        // Assert
        this.m_MockPipe.Verify(mock => mock.InvokeAsync(this.m_InputPort, this.m_OutputPort, this.m_MockServiceFactory.Object, It.IsAny<NextPipeHandleAsync>(), default), Times.Once());

        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockPipe.VerifyNoOtherCalls();
    }

    #endregion InvokePipeAsync Tests

    #region - - - - - - Nested Classes - - - - - -

    private class PipeProvider : IClosedGenericPipeProvider
    {

        #region - - - - - - Methods - - - - - -

        IPipe<TInputPort, TOutputPort> IClosedGenericPipeProvider.GetPipe<TInputPort, TOutputPort>(ServiceFactory serviceFactory)
            => (IPipe<TInputPort, TOutputPort>)serviceFactory(typeof(IPipe<TInputPort, TOutputPort>));

        #endregion Methods

    }

    #endregion Nested Classes

}
