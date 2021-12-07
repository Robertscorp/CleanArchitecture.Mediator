using CleanArchitecture.Services.Infrastructure;
using CleanArchitecture.Services.Pipeline;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Services.Tests.Unit.Infrastructure
{

    public class InteractorUseCaseElementTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<UseCaseElementHandleAsync> m_MockHandleDelegate = new();
        private readonly Mock<IUseCaseInteractor<object, object>> m_MockInteractor = new();
        private readonly Mock<IServiceProvider> m_MockServiceProvider = new();

        private readonly InteractorUseCaseElement m_Element;
        private readonly object m_InputPort = new();
        private readonly object m_OutputPort = new();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public InteractorUseCaseElementTests()
        {
            this.m_Element = new(this.m_MockServiceProvider.Object);

            _ = this.m_MockServiceProvider
                    .Setup(mock => mock.GetService(typeof(IUseCaseInteractor<object, object>)))
                    .Returns(this.m_MockInteractor.Object);
        }

        #endregion Constructors

        #region - - - - - - HandleAsync Tests - - - - - -

        [Fact]
        public async Task HandleAsync_InteractorDoesNotExist_DoesNotFailOrInvokeNextElementHandleDelegate()
        {
            // Arrange
            this.m_MockServiceProvider.Reset();

            // Act
            var _Exception = await Record.ExceptionAsync(() => this.m_Element.HandleAsync(this.m_InputPort, this.m_OutputPort, this.m_MockHandleDelegate.Object, default));

            // Assert
            _ = _Exception.Should().BeNull();

            this.m_MockHandleDelegate.Verify(mock => mock.Invoke(), Times.Never());
        }

        [Fact]
        public async Task HandleAsync_InteractorExists_InvokesUseCaseAsyncAndDoesNotInvokeNextElementHandleDelegate()
        {
            // Arrange

            // Act
            await this.m_Element.HandleAsync(this.m_InputPort, this.m_OutputPort, this.m_MockHandleDelegate.Object, default);

            // Assert
            this.m_MockHandleDelegate.Verify(mock => mock.Invoke(), Times.Never());
            this.m_MockInteractor.Verify(mock => mock.HandleAsync(this.m_InputPort, this.m_OutputPort, default), Times.Once());
        }

        #endregion HandleAsync Tests

    }

}
