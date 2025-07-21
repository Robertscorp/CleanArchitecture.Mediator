using CleanArchitecture.Mediator.Internal;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Unit.Internal
{

    public class InteractorInvocationPipeTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IInteractor<IInputPort<object>, object>> m_MockInteractor = new();
        private readonly Mock<IPipeHandle> m_MockNextPipeHandle = new();
        private readonly Mock<ServiceFactory> m_MockServiceFactory = new();

        private readonly IInputPort<object> m_InputPort = new Mock<IInputPort<object>>().Object;
        private readonly object m_OutputPort = new();
        private readonly IPipe m_Pipe = new InteractorInvocationPipe();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public InteractorInvocationPipeTests()
            => _ = this.m_MockServiceFactory
                    .Setup(mock => mock.Invoke(typeof(IInteractor<IInputPort<object>, object>)))
                    .Returns(this.m_MockInteractor.Object);

        #endregion Constructors

        #region - - - - - - InvokeAsync Tests - - - - - -

        [Fact]
        public async Task InvokeAsync_InteractorDoesNotExist_DoesNothing()
        {
            // Arrange
            this.m_MockServiceFactory.Reset();

            // Act
            await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_OutputPort, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

            // Assert
            this.m_MockInteractor.VerifyNoOtherCalls();
            this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task InvokeAsync_InteractorExists_InvokesInteractor()
        {
            // Arrange

            // Act
            await this.m_Pipe.InvokeAsync(this.m_InputPort, this.m_OutputPort, this.m_MockServiceFactory.Object, this.m_MockNextPipeHandle.Object, default);

            // Assert
            this.m_MockInteractor.Verify(mock => mock.HandleAsync(this.m_InputPort, this.m_OutputPort, this.m_MockServiceFactory.Object, default), Times.Once());

            this.m_MockInteractor.VerifyNoOtherCalls();
            this.m_MockNextPipeHandle.VerifyNoOtherCalls();
        }

        #endregion InvokeAsync Tests

    }

}
