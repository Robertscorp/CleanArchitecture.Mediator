using CleanArchitecture.Services.Extended.Infrastructure;
using CleanArchitecture.Services.Extended.Pipeline;
using CleanArchitecture.Services.Extended.Validation;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Services.Extended.Tests.Unit.Infrastructure
{

    public class InputPortValidatorUseCaseElementTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IServiceProvider> m_MockServiceProvdider = new();
        private readonly Mock<IValidationOutputPort<TestValidationResult>> m_MockValidationOutputPort = new();
        private readonly Mock<IUseCaseInputPortValidator<object, TestValidationResult>> m_MockValidator = new();

        private readonly InputPortValidatorUseCaseElement<TestValidationResult> m_Element;
        private readonly object m_InputPort = new();
        private readonly TestValidationResult m_ValidationResult = new();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public InputPortValidatorUseCaseElementTests()
        {
            this.m_Element = new(this.m_MockServiceProvdider.Object);

            _ = this.m_MockServiceProvdider
                    .Setup(mock => mock.GetService(typeof(IUseCaseInputPortValidator<object, TestValidationResult>)))
                    .Returns(this.m_MockValidator.Object);

            _ = this.m_MockValidator
                    .Setup(mock => mock.ValidateAsync(this.m_InputPort, default))
                    .Returns(Task.FromResult(this.m_ValidationResult));
        }

        #endregion Constructors

        #region - - - - - - TryOutputResultAsync Tests - - - - - -

        [Fact]
        public async Task TryOutputResultAsync_OutputPortDoesNotSupportValidation_ReturnsFalse()
        {
            // Arrange
            var _OutputPort = new object();

            // Act
            var _Actual = await this.m_Element.TryOutputResultAsync(this.m_InputPort, _OutputPort, default);

            // Assert
            _ = _Actual.Should().BeFalse();
        }

        [Fact]
        public async Task TryOutputResultAsync_ValidatorHasNotBeenRegistered_ReturnsFalse()
        {
            // Arrange
            this.m_MockServiceProvdider.Reset();

            // Act
            var _Actual = await this.m_Element.TryOutputResultAsync(this.m_InputPort, this.m_MockValidationOutputPort.Object, default);

            // Assert
            _ = _Actual.Should().BeFalse();

            this.m_MockValidationOutputPort.Verify(mock => mock.PresentValidationFailureAsync(It.IsAny<TestValidationResult>(), default), Times.Never());
        }

        [Fact]
        public async Task TryOutputResultAsync_ValidationSuccessful_ReturnsFalse()
        {
            // Arrange
            this.m_ValidationResult.IsValid = true;

            // Act
            var _Actual = await this.m_Element.TryOutputResultAsync(this.m_InputPort, this.m_MockValidationOutputPort.Object, default);

            // Assert
            _ = _Actual.Should().BeFalse();

            this.m_MockValidationOutputPort.Verify(mock => mock.PresentValidationFailureAsync(It.IsAny<TestValidationResult>(), default), Times.Never());
        }

        [Fact]
        public async Task TryOutputResultAsync_ValidationFails_PresentsValidationFailureAsyncAndReturnsTrue()
        {
            // Arrange

            // Act
            var _Actual = await this.m_Element.TryOutputResultAsync(this.m_InputPort, this.m_MockValidationOutputPort.Object, default);

            // Assert
            _ = _Actual.Should().BeTrue();

            this.m_MockValidationOutputPort.Verify(mock => mock.PresentValidationFailureAsync(It.IsAny<TestValidationResult>(), default), Times.Once());
        }

        #endregion TryOutputResultAsync Tests

        #region - - - - - - Nested Classes - - - - - -

        public class TestValidationResult : IValidationResult
        {

            #region - - - - - - IValidationResult Implementation - - - - - -

            public bool IsValid { get; set; }

            #endregion IValidationResult Implementation

        }

        #endregion Nested Classes

    }

}
