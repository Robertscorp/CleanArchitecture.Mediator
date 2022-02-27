using CleanArchitecture.Mediator.Authentication;
using CleanArchitecture.Mediator.Infrastructure;
using CleanArchitecture.Mediator.Pipeline;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Integration.Infrastructure
{

    public class UseCaseInvokerTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IUseCaseAuthorisationEnforcer<InputPort, AuthorisationResult>> m_MockAuthEnforcer = new();
        private readonly Mock<IUseCaseBusinessRuleValidator<InputPort, ValidationResult>> m_MockBusinessRuleValidator = new();
        private readonly Mock<IAuthenticatedClaimsPrincipalProvider> m_MockClaimsPrincipalProvider = new();
        private readonly Mock<IEmptyOutputPort> m_MockEmptyOutputPort = new();
        private readonly Mock<IUseCaseInteractor<InputPort, IEmptyOutputPort>> m_MockEmptyOutputPortInteractor = new();
        private readonly Mock<IEverythingOutputPort> m_MockEverythingOutputPort = new();
        private readonly Mock<IUseCaseInputPortValidator<InputPort, ValidationResult>> m_MockInputPortValidator = new();
        private readonly Mock<UseCaseServiceResolver> m_MockServiceResolver = new();

        private readonly AuthorisationResult m_AuthResult = new() { IsAuthorised = true };
        private readonly ValidationResult m_BusinessRuleValidationResult = new() { IsValid = true };
        private readonly InputPort m_InputPort = new();
        private readonly IUseCaseInvoker m_UseCaseInvoker;
        private readonly ValidationResult m_InputPortValidationResult = new() { IsValid = true };

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public UseCaseInvokerTests()
        {
            this.m_UseCaseInvoker = new UseCaseInvoker(this.m_MockServiceResolver.Object);

            _ = this.m_MockAuthEnforcer
                    .Setup(mock => mock.CheckAuthorisationAsync(this.m_InputPort, default))
                    .Returns(Task.FromResult(this.m_AuthResult));

            _ = this.m_MockBusinessRuleValidator
                    .Setup(mock => mock.ValidateAsync(this.m_InputPort, default))
                    .Returns(Task.FromResult(this.m_BusinessRuleValidationResult));

            _ = this.m_MockClaimsPrincipalProvider
                    .Setup(mock => mock.AuthenticatedClaimsPrincipal)
                    .Returns(new ClaimsPrincipal());

            _ = this.m_MockInputPortValidator
                    .Setup(mock => mock.ValidateAsync(this.m_InputPort, default))
                    .Returns(Task.FromResult(this.m_InputPortValidationResult));

            _ = this.m_MockServiceResolver
                    .Setup(mock => mock.Invoke(typeof(IEnumerable<IUseCasePipe>)))
                    .Returns(new List<IUseCasePipe>()
                    {
                        new AuthenticationPipe(this.m_MockServiceResolver.Object),
                        new AuthorisationPipe<AuthorisationResult>(this.m_MockServiceResolver.Object),
                        new InputPortValidationPipe<ValidationResult>(this.m_MockServiceResolver.Object),
                        new BusinessRuleValidationPipe<ValidationResult>(this.m_MockServiceResolver.Object),
                        new InteractorInvocationPipe(this.m_MockServiceResolver.Object)
                    });

            _ = this.m_MockServiceResolver
                    .Setup(mock => mock.Invoke(typeof(IUseCaseAuthorisationEnforcer<InputPort, AuthorisationResult>)))
                    .Returns(this.m_MockAuthEnforcer.Object);

            _ = this.m_MockServiceResolver
                    .Setup(mock => mock.Invoke(typeof(IUseCaseBusinessRuleValidator<InputPort, ValidationResult>)))
                    .Returns(this.m_MockBusinessRuleValidator.Object);

            _ = this.m_MockServiceResolver
                    .Setup(mock => mock.Invoke(typeof(IAuthenticatedClaimsPrincipalProvider)))
                    .Returns(this.m_MockClaimsPrincipalProvider.Object);

            _ = this.m_MockServiceResolver
                    .Setup(mock => mock.Invoke(typeof(IUseCaseInteractor<InputPort, IEmptyOutputPort>)))
                    .Returns(this.m_MockEmptyOutputPortInteractor.Object);

            _ = this.m_MockServiceResolver
                    .Setup(mock => mock.Invoke(typeof(IUseCaseInputPortValidator<InputPort, ValidationResult>)))
                    .Returns(this.m_MockInputPortValidator.Object);

        }

        #endregion Constructors

        #region - - - - - - InvokeUseCaseAsync Tests - - - - - -

        [Fact]
        public async Task InvokeUseCaseAsync_EmptyOutputPort_InvokesUseCaseAsync()
        {
            // Arrange

            // Act
            await this.m_UseCaseInvoker.InvokeUseCaseAsync(this.m_InputPort, this.m_MockEmptyOutputPort.Object, default);

            // Assert
            this.m_MockEmptyOutputPortInteractor.Verify(mock => mock.HandleAsync(this.m_InputPort, this.m_MockEmptyOutputPort.Object, default), Times.Once());
        }

        [Fact]
        public async Task InvokeUseCaseAsync_EverythingOutputPortWithAuthenticationFailure_PresentsUnauthenticatedAsync()
        {
            // Arrange
            this.m_MockClaimsPrincipalProvider.Reset();

            // Act
            await this.m_UseCaseInvoker.InvokeUseCaseAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, default);

            // Assert
            this.m_MockAuthEnforcer.Verify(mock => mock.CheckAuthorisationAsync(this.m_InputPort, default), Times.Never());
            this.m_MockBusinessRuleValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, default), Times.Never());
            this.m_MockClaimsPrincipalProvider.Verify(mock => mock.AuthenticatedClaimsPrincipal, Times.Once());
            this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, default), Times.Never());

            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentUnauthenticatedAsync(default), Times.Once());
            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentValidationFailureAsync(this.m_InputPortValidationResult, default), Times.Never());
            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentBusinessRuleValidationFailureAsync(this.m_BusinessRuleValidationResult, default), Times.Never());
            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentUnauthorisedAsync(this.m_AuthResult, default), Times.Never());
        }

        [Fact]
        public async Task InvokeUseCaseAsync_EverythingOutputPortWithAuthorisationFailure_PresentsUnauthorisedAsync()
        {
            // Arrange
            this.m_AuthResult.IsAuthorised = false;

            // Act
            await this.m_UseCaseInvoker.InvokeUseCaseAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, default);

            // Assert
            this.m_MockAuthEnforcer.Verify(mock => mock.CheckAuthorisationAsync(this.m_InputPort, default), Times.Once());
            this.m_MockBusinessRuleValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, default), Times.Never());
            this.m_MockClaimsPrincipalProvider.Verify(mock => mock.AuthenticatedClaimsPrincipal, Times.Once());
            this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, default), Times.Never());

            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentUnauthenticatedAsync(default), Times.Never());
            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentValidationFailureAsync(this.m_InputPortValidationResult, default), Times.Never());
            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentBusinessRuleValidationFailureAsync(this.m_BusinessRuleValidationResult, default), Times.Never());
            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentUnauthorisedAsync(this.m_AuthResult, default), Times.Once());
        }

        [Fact]
        public async Task InvokeUseCaseAsync_EverythingOutputPortWithInputPortValidationFailure_PresentsValidationFailureAsync()
        {
            // Arrange
            this.m_InputPortValidationResult.IsValid = false;

            // Act
            await this.m_UseCaseInvoker.InvokeUseCaseAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, default);

            // Assert
            this.m_MockAuthEnforcer.Verify(mock => mock.CheckAuthorisationAsync(this.m_InputPort, default), Times.Once());
            this.m_MockBusinessRuleValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, default), Times.Never());
            this.m_MockClaimsPrincipalProvider.Verify(mock => mock.AuthenticatedClaimsPrincipal, Times.Once());
            this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, default), Times.Once());

            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentUnauthenticatedAsync(default), Times.Never());
            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentValidationFailureAsync(this.m_InputPortValidationResult, default), Times.Once());
            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentBusinessRuleValidationFailureAsync(this.m_BusinessRuleValidationResult, default), Times.Never());
            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentUnauthorisedAsync(this.m_AuthResult, default), Times.Never());
        }

        [Fact]
        public async Task InvokeUseCaseAsync_EverythingOutputPortWithBusinessRuleValidationFailure_PresentsBusinessRuleValidationFailureAsync()
        {
            // Arrange
            this.m_BusinessRuleValidationResult.IsValid = false;

            // Act
            await this.m_UseCaseInvoker.InvokeUseCaseAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, default);

            // Assert
            this.m_MockAuthEnforcer.Verify(mock => mock.CheckAuthorisationAsync(this.m_InputPort, default), Times.Once());
            this.m_MockBusinessRuleValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, default), Times.Once());
            this.m_MockClaimsPrincipalProvider.Verify(mock => mock.AuthenticatedClaimsPrincipal, Times.Once());
            this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, default), Times.Once());

            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentUnauthenticatedAsync(default), Times.Never());
            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentValidationFailureAsync(this.m_InputPortValidationResult, default), Times.Never());
            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentBusinessRuleValidationFailureAsync(this.m_BusinessRuleValidationResult, default), Times.Once());
            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentUnauthorisedAsync(this.m_AuthResult, default), Times.Never());
        }

        #endregion InvokeUseCaseAsync Tests

        #region - - - - - - Nested Classes - - - - - -

        public class AuthorisationResult : IAuthorisationResult
        {

            #region - - - - - - Properties - - - - - -

            public bool IsAuthorised { get; set; }

            #endregion Properties

        }

        public interface IEmptyOutputPort { }

        public interface IEverythingOutputPort :
            IAuthenticationOutputPort,
            IAuthorisationOutputPort<AuthorisationResult>,
            IValidationOutputPort<ValidationResult>,
            IBusinessRuleValidationOutputPort<ValidationResult>
        { }

        public class InputPort :
            IUseCaseInputPort<IEmptyOutputPort>,
            IUseCaseInputPort<IEverythingOutputPort>
        { }

        public class ValidationResult : IValidationResult
        {

            #region - - - - - - Properties - - - - - -

            public bool IsValid { get; set; }

            #endregion Properties

        }

        #endregion Nested Classes

    }

}
