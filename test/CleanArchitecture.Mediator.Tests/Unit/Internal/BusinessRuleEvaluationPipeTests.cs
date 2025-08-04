using CleanArchitecture.Mediator.Internal;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Unit.Internal;

public class BusinessRuleEvaluationPipeTests
{

    #region - - - - - - Fields - - - - - -

    private readonly Mock<ContinuationBehaviour> m_MockContinuation = new();
    private readonly Mock<IBusinessRuleEvaluator<IInputPort<object>, object>> m_MockEvaluator = new();
    private readonly Mock<NextPipeHandleAsync> m_MockNextPipeHandle = new();
    private readonly Mock<ServiceFactory> m_MockServiceFactory = new();

    private readonly IInputPort<object> m_InputPort = new Mock<IInputPort<object>>().Object;
    private readonly object m_OutputPort = new();
    private readonly IPipe m_Pipe = new BusinessRuleEvaluationPipe();


    #endregion Fields

    #region - - - - - - Constructors - - - - - -

    public BusinessRuleEvaluationPipeTests()
    {
        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(IBusinessRuleEvaluator<IInputPort<object>, object>)))
                .Returns(this.m_MockEvaluator.Object);

        _ = this.m_MockEvaluator
                .Setup(mock => mock.EvaluateAsync(this.m_InputPort, this.m_OutputPort, this.m_MockServiceFactory.Object, default))
                .Returns(() => Task.FromResult(this.m_MockContinuation.Object));
    }

    #endregion Constructors

    #region - - - - - - InvokeAsync Tests - - - - - -

    [Fact]
    public async Task InvokeAsync_EvaluatorHasNotBeenRegistered_MovesToNextPipe()
    {
        // Arrange
        this.m_MockServiceFactory.Reset();

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_OutputPort, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockNextPipeHandle.Verify(mock => mock.Invoke(), Times.Once());

        this.m_MockContinuation.VerifyNoOtherCalls();
        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockEvaluator.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_EvaluatorHasBeenRegistered_InvokesContinuationFromEvaluator()
    {
        // Arrange

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_OutputPort, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockContinuation.Verify(mock => mock.HandleAsync(this.m_MockNextPipeHandle.Object, default), Times.Once());
        this.m_MockEvaluator.Verify(mock => mock.EvaluateAsync(this.m_InputPort, this.m_OutputPort, this.m_MockServiceFactory.Object, default), Times.Once());

        this.m_MockContinuation.VerifyNoOtherCalls();
        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockEvaluator.VerifyNoOtherCalls();
    }

    #endregion InvokeAsync Tests

}
