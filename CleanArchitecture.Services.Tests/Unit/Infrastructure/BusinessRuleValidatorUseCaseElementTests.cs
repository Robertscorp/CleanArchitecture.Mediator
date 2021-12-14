using CleanArchitecture.Services.Infrastructure;
using CleanArchitecture.Services.Pipeline;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Services.Tests.Unit.Infrastructure
{

    public class BusinessRuleValidatorUseCaseElementTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IBusinessRuleValidationOutputPort<TestValidationResult>> m_MockOutputPort = new();
        private readonly Mock<UseCaseElementHandleAsync> m_MockNextHandleDelegate = new();
        private readonly Mock<UseCaseServiceResolver> m_MockServiceResolver = new();
        private readonly Mock<IUseCaseBusinessRuleValidator<TestInputPort, TestValidationResult>> m_MockValidator = new();

        private readonly BusinessRuleValidatorUseCaseElement<TestValidationResult> m_Element;
        private readonly TestInputPort m_InputPort = new();
        private readonly TestValidationResult m_ValidationResult = new();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public BusinessRuleValidatorUseCaseElementTests()
        {
            this.m_Element = new(this.m_MockServiceResolver.Object);

            _ = this.m_MockServiceResolver
                    .Setup(mock => mock.Invoke(typeof(IUseCaseBusinessRuleValidator<TestInputPort, TestValidationResult>)))
                    .Returns(this.m_MockValidator.Object);

            _ = this.m_MockValidator
                    .Setup(mock => mock.ValidateAsync(this.m_InputPort, default))
                    .Returns(Task.FromResult(this.m_ValidationResult));
        }

        #endregion Constructors

        #region - - - - - - HandleAsync Tests - - - - - -

        [Fact]
        public async Task HandleAsync_InputPortDoesNotSupportValidation_InvokesNextHandleDelegate()
        {
            // Arrange
            var _InputPort = new object();

            // Act
            await this.m_Element.HandleAsync(_InputPort, this.m_MockOutputPort.Object, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Once());
        }

        [Fact]
        public async Task HandleAsync_OutputPortDoesNotSupportValidation_InvokesNextHandleDelegate()
        {
            // Arrange
            var _OutputPort = new object();

            // Act
            await this.m_Element.HandleAsync(this.m_InputPort, _OutputPort, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Once());
        }

        [Fact]
        public async Task HandleAsync_ValidatorHasNotBeenRegistered_InvokesNextHandleDelegate()
        {
            // Arrange
            this.m_MockServiceResolver.Reset();

            // Act
            await this.m_Element.HandleAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Once());
            this.m_MockOutputPort.Verify(mock => mock.PresentBusinessRuleValidationFailureAsync(It.IsAny<TestValidationResult>(), default), Times.Never());
        }

        [Fact]
        public async Task HandleAsync_ValidationSuccessful_InvokesNextHandleDelegate()
        {
            // Arrange
            this.m_ValidationResult.IsValid = true;

            // Act
            await this.m_Element.HandleAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Once());
            this.m_MockOutputPort.Verify(mock => mock.PresentBusinessRuleValidationFailureAsync(It.IsAny<TestValidationResult>(), default), Times.Never());
        }

        [Fact]
        public async Task HandleAsync_ValidationFails_PresentsValidationFailureAsync()
        {
            // Arrange

            // Act
            await this.m_Element.HandleAsync(this.m_InputPort, this.m_MockOutputPort.Object, this.m_MockNextHandleDelegate.Object, default);

            // Assert
            this.m_MockNextHandleDelegate.Verify(mock => mock.Invoke(), Times.Never());
            this.m_MockOutputPort.Verify(mock => mock.PresentBusinessRuleValidationFailureAsync(It.IsAny<TestValidationResult>(), default), Times.Once());
        }

        #endregion HandleAsync Tests

        #region - - - - - - Nested Classes - - - - - -

        public class TestInputPort : IUseCaseInputPort<IBusinessRuleValidationOutputPort<TestValidationResult>> { }

        public class TestValidationResult : IValidationResult
        {

            #region - - - - - - IValidationResult Implementation - - - - - -

            public bool IsValid { get; set; }

            #endregion IValidationResult Implementation

        }

        #endregion Nested Classes

    }

}
