using CleanArchitecture.Mediator.Internal;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Unit.Internal;

public class AuthorisationPipeTests
{

    #region - - - - - - Fields - - - - - -

    private readonly Mock<IAuthorisationEnforcer<IInputPort<IAuthorisationOutputPort<object>>, object>> m_MockAuthorisationEnforcer = new();
    private readonly Mock<NextPipeHandleAsync> m_MockNextPipeHandle = new();
    private readonly Mock<IAuthorisationOutputPort<object>> m_MockOutputPort = new();
    private readonly Mock<ServiceFactory> m_MockServiceFactory = new();

    private readonly IInputPort<IAuthorisationOutputPort<object>> m_InputPort = new Mock<IInputPort<IAuthorisationOutputPort<object>>>().Object;
    private readonly IPipe<IInputPort<IAuthorisationOutputPort<object>>, IAuthorisationOutputPort<object>> m_Pipe
        = new AuthorisationPipe<IInputPort<IAuthorisationOutputPort<object>>, IAuthorisationOutputPort<object>, object>();

    private object? m_AuthorisationFailure;
    private bool m_IsAuthorised = true;

    #endregion Fields

    #region - - - - - - Constructors - - - - - -

    public AuthorisationPipeTests()
    {
        var _AuthorisationFailure = new object();

        _ = this.m_MockAuthorisationEnforcer
                .Setup(mock => mock.IsAuthorisedAsync(this.m_InputPort, out _AuthorisationFailure, this.m_MockServiceFactory.Object, default))
                .Returns(() =>
                {
                    this.m_AuthorisationFailure = _AuthorisationFailure;
                    return Task.FromResult(this.m_IsAuthorised);
                });

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(IAuthorisationEnforcer<IInputPort<IAuthorisationOutputPort<object>>, object>)))
                .Returns(this.m_MockAuthorisationEnforcer.Object);
    }

    #endregion Constructors

    #region - - - - - - InvokeAsync Tests - - - - - -

    [Fact]
    public async Task InvokeAsync_EnforcerHasNotBeenRegistered_MovesToNextPipe()
    {
        // Arrange
        this.m_MockServiceFactory.Reset();

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockNextPipeHandle.Verify(mock => mock.Invoke(), Times.Once());

        this.m_MockAuthorisationEnforcer.VerifyNoOtherCalls();
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
        this.m_MockAuthorisationEnforcer.Verify(mock => mock.IsAuthorisedAsync(this.m_InputPort, out this.m_AuthorisationFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockNextPipeHandle.Verify(mock => mock.Invoke(), Times.Once());

        this.m_MockAuthorisationEnforcer.VerifyNoOtherCalls();
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
        this.m_MockAuthorisationEnforcer.Verify(mock => mock.IsAuthorisedAsync(this.m_InputPort, out this.m_AuthorisationFailure, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockOutputPort.Verify(mock => mock.PresentAuthorisationFailureAsync(this.m_AuthorisationFailure!, default));

        this.m_MockAuthorisationEnforcer.VerifyNoOtherCalls();
        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
    }

    #endregion InvokeAsync Tests

}
