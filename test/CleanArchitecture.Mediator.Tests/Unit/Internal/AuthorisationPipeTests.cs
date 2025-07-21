using CleanArchitecture.Mediator.Internal;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Unit.Internal;

public class AuthorisationPipeTests
{

    #region - - - - - - Fields - - - - - -

    private readonly Mock<IAuthorisationEnforcer<TestInputPort, ITestOutputPort>> m_MockAuthorisationEnforcer = new();
    private readonly Mock<IPipeHandle> m_MockNextPipeHandle = new();
    private readonly Mock<ITestOutputPort> m_MockOutputPort = new();
    private readonly Mock<ServiceFactory> m_MockServiceFactory = new();

    private readonly TestInputPort m_InputPort = new();
    private readonly IPipe m_Pipe = new AuthorisationPipe();

    private bool m_AuthResult;

    #endregion Fields

    #region - - - - - - Constructors - - - - - -

    public AuthorisationPipeTests()
    {
        _ = this.m_MockAuthorisationEnforcer
                .Setup(mock => mock.HandleAuthorisationAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, default))
                .Returns(() => Task.FromResult(this.m_AuthResult));

        _ = this.m_MockServiceFactory
                .Setup(mock => mock.Invoke(typeof(IAuthorisationEnforcer<TestInputPort, ITestOutputPort>)))
                .Returns(this.m_MockAuthorisationEnforcer.Object);
    }

    #endregion Constructors

    #region - - - - - - InvokeAsync Tests - - - - - -

    [Fact]
    public async Task InvokeAsync_OutputPortDoesNotSupportAuthorisation_MovesToNextPipe()
    {
        // Arrange
        var _OutputPort = new object();

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, _OutputPort, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockNextPipeHandle.Verify(mock => mock.InvokePipeAsync(this.m_InputPort, _OutputPort, this.m_MockServiceFactory.Object, default), Times.Once());

        this.m_MockAuthorisationEnforcer.VerifyNoOtherCalls();
        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_EnforcerHasNotBeenRegistered_MovesToNextPipe()
    {
        // Arrange
        this.m_MockServiceFactory.Reset();

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockNextPipeHandle.Verify(mock => mock.InvokePipeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Once());

        this.m_MockAuthorisationEnforcer.VerifyNoOtherCalls();
        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_AuthorisationSuccessful_MovesToNextPipe()
    {
        // Arrange
        this.m_AuthResult = true;

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockAuthorisationEnforcer.Verify(mock => mock.HandleAuthorisationAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Once());
        this.m_MockNextPipeHandle.Verify(mock => mock.InvokePipeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Once());

        this.m_MockAuthorisationEnforcer.VerifyNoOtherCalls();
        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_AuthorisationFails_StopsWithAuthorisationFailure()
    {
        // Arrange

        // Act
        await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

        // Assert
        this.m_MockAuthorisationEnforcer.Verify(mock => mock.HandleAuthorisationAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockServiceFactory.Object, default), Times.Once());

        this.m_MockAuthorisationEnforcer.VerifyNoOtherCalls();
        this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        this.m_MockOutputPort.VerifyNoOtherCalls();
    }

    #endregion InvokeAsync Tests

    #region - - - - - - Nested Classes - - - - - -

    public class TestInputPort : IInputPort<ITestOutputPort> { }

    public interface ITestOutputPort { }

    #endregion Nested Classes

}
