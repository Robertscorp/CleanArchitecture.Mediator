using CleanArchitecture.Mediator.Internal;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Unit.Internal;

public class InputPortValidationPipeTests
{

    #region - - - - - - Fields - - - - - -

    private readonly Mock<NextPipeHandleAsync> m_MockNextPipeHandle = new();
    private readonly Mock<IInputPortValidationOutputPort<IInputPortValidationResult>> m_MockOutputPort = new();
    private readonly Mock<ServiceFactory> m_MockServiceFactory = new();
    private readonly Mock<IInputPortValidationResult> m_MockValidationResult = new();
    private readonly Mock<IInputPortValidator<IInputPort<IInputPortValidationOutputPort<IInputPortValidationResult>>, IInputPortValidationResult>> m_MockValidator = new();

    private readonly IInputPort<IInputPortValidationOutputPort<IInputPortValidationResult>> m_InputPort = new Mock<IInputPort<IInputPortValidationOutputPort<IInputPortValidationResult>>>().Object;
    private readonly IPipe<IInputPort<IInputPortValidationOutputPort<IInputPortValidationResult>>, IInputPortValidationOutputPort<IInputPortValidationResult>> m_Pipe
        = new InputPortValidationPipe<IInputPort<IInputPortValidationOutputPort<IInputPortValidationResult>>, IInputPortValidationOutputPort<IInputPortValidationResult>, IInputPortValidationResult>();

    #endregion Fields

    #region - - - - - - Constructors - - - - - -

    public InputPortValidationPipeTests()
    {
        _ = this.m_MockValidationResult.Setup(mock => mock.IsValid).Returns(true);

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(IInputPortValidator<IInputPort<IInputPortValidationOutputPort<IInputPortValidationResult>>, IInputPortValidationResult>)))
                .Returns(this.m_MockValidator.Object);

        _ = this.m_MockValidator
                .Setup(mock => mock.ValidateAsync(this.m_InputPort, this.m_MockServiceFactory.Object, default))
                .Returns(Task.FromResult(this.m_MockValidationResult.Object));
    }

    #endregion Constructors

    #region - - - - - - InvokeAsync Tests - - - - - -

    [Fact]
    public async Task InvokeAsync_InputPortValidatorHasNotBeenRegistered_MovesToNextPipe()
    {
        // Arrange
        this.m_MockServiceFactory.Reset();

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockNextPipeHandle.Verify(mock => mock.Invoke(), Times.Once());

        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
        this.m_MockValidator.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_InputPortIsInvalid_StopsWithInputPortValidationFailure()
    {
        // Arrange
        this.m_MockValidationResult.Reset();

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockValidationResult.Verify(mock => mock.IsValid, Times.Once());
        this.m_MockOutputPort.Verify(mock => mock.PresentInputPortValidationFailureAsync(this.m_MockValidationResult.Object, default), Times.Once());
        this.m_MockValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, this.m_MockServiceFactory.Object, default), Times.Once());

        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
        this.m_MockValidator.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_InputPortIsValid_MovesToNextPipe()
    {
        // Arrange

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockValidationResult.Verify(mock => mock.IsValid, Times.Once());
        this.m_MockValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockNextPipeHandle.Verify(mock => mock.Invoke(), Times.Once());

        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
        this.m_MockValidator.VerifyNoOtherCalls();
    }

    #endregion InvokeAsync Tests

}
