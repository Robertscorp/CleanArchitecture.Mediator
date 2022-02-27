using CleanArchitecture.Mediator.Infrastructure;
using CleanArchitecture.Mediator.Pipeline;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Unit.Infrastructure
{

    public class BusinessRuleValidationPipeTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IBusinessRuleValidationOutputPort<IValidationResult>> m_MockOutputPort = new();
        private readonly Mock<UseCasePipeHandleAsync> m_MockNextHandleDelegate = new();
        private readonly Mock<UseCaseServiceResolver> m_MockServiceResolver = new();
        private readonly Mock<IValidationResult> m_MockValidationResult = new();
        private readonly Mock<IUseCaseBusinessRuleValidator<TestInputPort, IValidationResult>> m_MockValidator = new();

        private readonly TestInputPort m_InputPort = new();
        private readonly IUseCasePipe m_Pipe;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public BusinessRuleValidationPipeTests()
        {
            this.m_Pipe = new BusinessRuleValidationPipe<IValidationResult>(this.m_MockServiceResolver.Object);

            _ = this.m_MockServiceResolver
                    .Setup(mock => mock.Invoke(typeof(IUseCaseBusinessRuleValidator<TestInputPort, IValidationResult>)))
                    .Returns(this.m_MockValidator.Object);

            _ = this.m_MockValidator
                    .Setup(mock => mock.ValidateAsync(this.m_InputPort, default))
                    .Returns(Task.FromResult(this.m_MockValidationResult.Object));
        }

        #endregion Constructors

        #region - - - - - - HandleAsync Tests - - - - - -

        [Fact]
        public async Task HandleAsync_InputPortDoesNotSupportValidation_InvokesNextHandleDelegate()
        {
            // Arrange
            var _InputPort = new object();

            // Act
            await this.m_Pipe.HandleAsync(_InputPort, this.m_MockOutputPort.Object, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Once());
        }

        [Fact]
        public async Task HandleAsync_OutputPortDoesNotSupportValidation_InvokesNextHandleDelegate()
        {
            // Arrange
            var _OutputPort = new object();

            // Act
            await this.m_Pipe.HandleAsync(this.m_InputPort, _OutputPort, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Once());
        }

        [Fact]
        public async Task HandleAsync_ValidatorHasNotBeenRegistered_InvokesNextHandleDelegate()
        {
            // Arrange
            this.m_MockServiceResolver.Reset();

            // Act
            await this.m_Pipe.HandleAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Once());
            this.m_MockOutputPort.Verify(mock => mock.PresentBusinessRuleValidationFailureAsync(It.IsAny<IValidationResult>(), default), Times.Never());
        }

        [Fact]
        public async Task HandleAsync_ValidationSuccessful_InvokesNextHandleDelegate()
        {
            // Arrange
            _ = this.m_MockValidationResult
                    .Setup(mock => mock.IsValid)
                    .Returns(true);

            // Act
            await this.m_Pipe.HandleAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Once());
            this.m_MockOutputPort.Verify(mock => mock.PresentBusinessRuleValidationFailureAsync(It.IsAny<IValidationResult>(), default), Times.Never());
        }

        [Fact]
        public async Task HandleAsync_ValidationFails_PresentsValidationFailureAsync()
        {
            // Arrange

            // Act
            await this.m_Pipe.HandleAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Never());
            this.m_MockOutputPort.Verify(mock => mock.PresentBusinessRuleValidationFailureAsync(It.IsAny<IValidationResult>(), default), Times.Once());
        }

        #endregion HandleAsync Tests

        #region - - - - - - Nested Classes - - - - - -

        public class TestInputPort : IUseCaseInputPort<IBusinessRuleValidationOutputPort<IValidationResult>> { }

        #endregion Nested Classes

    }

}
