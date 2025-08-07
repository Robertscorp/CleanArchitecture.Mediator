using CleanArchitecture.Mediator.Internal;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Unit.Internal;

public class AuthorisationPolicyValidationPipeTests
{

    #region - - - - - - Fields - - - - - -

    private readonly Mock<IAuthorisationPolicyValidator<IInputPort<IAuthorisationPolicyFailureOutputPort<object>>, object>> m_MockAuthorisationPolicyValidator = new();
    private readonly Mock<NextPipeHandleAsync> m_MockNextPipeHandle = new();
    private readonly Mock<IAuthorisationPolicyFailureOutputPort<object>> m_MockOutputPort = new();
    private readonly Mock<ServiceFactory> m_MockServiceFactory = new();

    private readonly IInputPort<IAuthorisationPolicyFailureOutputPort<object>> m_InputPort = new Mock<IInputPort<IAuthorisationPolicyFailureOutputPort<object>>>().Object;
    private readonly IPipe<IInputPort<IAuthorisationPolicyFailureOutputPort<object>>, IAuthorisationPolicyFailureOutputPort<object>> m_Pipe
        = new AuthorisationPolicyValidationPipe<IInputPort<IAuthorisationPolicyFailureOutputPort<object>>, IAuthorisationPolicyFailureOutputPort<object>, object>();

    private object? m_AuthorisationPolicyFailure;
    private bool m_IsAuthorised = true;

    #endregion Fields

    #region - - - - - - Constructors - - - - - -

    public AuthorisationPolicyValidationPipeTests()
    {
        var _AuthorisationPolicyFailure = new object();

        _ = this.m_MockAuthorisationPolicyValidator
                .Setup(mock => mock.ValidateAsync(this.m_InputPort, out _AuthorisationPolicyFailure, this.m_MockServiceFactory.Object, default))
                .Returns(() =>
                {
                    this.m_AuthorisationPolicyFailure = _AuthorisationPolicyFailure;
                    return Task.FromResult(this.m_IsAuthorised);
                });

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(IAuthorisationPolicyValidator<IInputPort<IAuthorisationPolicyFailureOutputPort<object>>, object>)))
                .Returns(this.m_MockAuthorisationPolicyValidator.Object);
    }

    #endregion Constructors

    #region - - - - - - InvokeAsync Tests - - - - - -

    [Fact]
    public async Task InvokeAsync_ValidatorHasNotBeenRegistered_MovesToNextPipe()
    {
        // Arrange
        this.m_MockServiceFactory.Reset();

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockNextPipeHandle.Verify(mock => mock.Invoke(), Times.Once());

        this.m_MockAuthorisationPolicyValidator.VerifyNoOtherCalls();
        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_AuthorisationSuccessful_MovesToNextPipe()
    {
        // Arrange

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockAuthorisationPolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_AuthorisationPolicyFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockNextPipeHandle.Verify(mock => mock.Invoke(), Times.Once());

        this.m_MockAuthorisationPolicyValidator.VerifyNoOtherCalls();
        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_AuthorisationFails_StopsWithAuthorisationFailure()
    {
        // Arrange
        this.m_IsAuthorised = false;

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockAuthorisationPolicyValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, out this.m_AuthorisationPolicyFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockOutputPort.Verify(mock => mock.PresentAuthorisationPolicyFailureAsync(this.m_AuthorisationPolicyFailure!, default));

        this.m_MockAuthorisationPolicyValidator.VerifyNoOtherCalls();
        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
    }

    #endregion InvokeAsync Tests

}
