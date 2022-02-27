using CleanArchitecture.Services.Infrastructure;
using CleanArchitecture.Services.Pipeline;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Services.Tests.Unit.Infrastructure
{

    public class InteractorUseCaseElementTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IUseCaseInteractor<IUseCaseInputPort<object>, object>> m_MockInteractor = new();
        private readonly Mock<UseCaseElementHandleAsync> m_MockNextHandleDelegate = new();
        private readonly Mock<UseCaseServiceResolver> m_MockServiceResolver = new();

        private readonly IUseCaseElement m_Element;
        private readonly IUseCaseInputPort<object> m_InputPort = new Mock<IUseCaseInputPort<object>>().Object;
        private readonly object m_OutputPort = new();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public InteractorUseCaseElementTests()
        {
            this.m_Element = new InteractorUseCaseElement(this.m_MockServiceResolver.Object);

            _ = this.m_MockServiceResolver
                    .Setup(mock => mock.Invoke(typeof(IUseCaseInteractor<IUseCaseInputPort<object>, object>)))
                    .Returns(this.m_MockInteractor.Object);

        }

        #endregion Constructors

        #region - - - - - - HandleAsync Tests - - - - - -

        [Fact]
        public async Task HandleAsync_InteractorDoesNotExist_DoesNothing()
        {
            // Arrange
            this.m_MockServiceResolver.Reset();

            // Act
            var _Exception = await Record.ExceptionAsync(() => this.m_Element.HandleAsync(this.m_InputPort, this.m_OutputPort, this.m_MockNextHandleDelegate.Object, default));

            // Assert
            _ = _Exception.Should().BeNull();

            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Never());
        }

        [Fact]
        public async Task HandleAsync_InteractorExists_InvokesUseCaseAsync()
        {
            // Arrange

            // Act
            await this.m_Element.HandleAsync(this.m_InputPort, this.m_OutputPort, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockInteractor.Verify(mock => mock.HandleAsync(this.m_InputPort, this.m_OutputPort, default), Times.Once());
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Never());
        }

        #endregion HandleAsync Tests

    }

}
