using CleanArchitecture.Mediator.Internal;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Unit.Internal;

public class InputPortValidationPipeTests
{

    #region - - - - - - Fields - - - - - -

    private readonly Mock<NextPipeHandleAsync> m_MockNextPipeHandle = new();
    private readonly Mock<IInputPortValidationOutputPort<object>> m_MockOutputPort = new();
    private readonly Mock<ServiceFactory> m_MockServiceFactory = new();
    private readonly Mock<IInputPortValidator<IInputPort<IInputPortValidationOutputPort<object>>, object>> m_MockValidator = new();

    private readonly IInputPort<IInputPortValidationOutputPort<object>> m_InputPort = new Mock<IInputPort<IInputPortValidationOutputPort<object>>>().Object;
    private readonly IPipe<IInputPort<IInputPortValidationOutputPort<object>>, IInputPortValidationOutputPort<object>> m_Pipe
        = new InputPortValidationPipe<IInputPort<IInputPortValidationOutputPort<object>>, IInputPortValidationOutputPort<object>, object>();

    private bool m_IsInputPortValid = true;
    private object? m_ValidationFailure;

    #endregion Fields

    #region - - - - - - Constructors - - - - - -

    public InputPortValidationPipeTests()
    {
        var _ValidationFailure = new object();

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(IInputPortValidator<IInputPort<IInputPortValidationOutputPort<object>>, object>)))
                .Returns(this.m_MockValidator.Object);

        _ = this.m_MockValidator
                .Setup(mock => mock.ValidateAsync(this.m_InputPort, out _ValidationFailure, this.m_MockServiceFactory.Object, default))
                .Returns(() =>
                {
                    this.m_ValidationFailure = _ValidationFailure;
                    return Task.FromResult(this.m_IsInputPortValid);
                });
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
        this.m_IsInputPortValid = false;

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockOutputPort.Verify(mock => mock.PresentInputPortValidationFailureAsync(this.m_ValidationFailure!, default), Times.Once());
        this.m_MockValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_ValidationFailure, this.m_MockServiceFactory.Object, default), Times.Once());

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
        this.m_MockValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_ValidationFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockNextPipeHandle.Verify(mock => mock.Invoke(), Times.Once());

        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
        this.m_MockValidator.VerifyNoOtherCalls();
    }

    #endregion InvokeAsync Tests

}
