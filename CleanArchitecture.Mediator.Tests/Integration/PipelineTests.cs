using CleanArchitecture.Mediator.Configuration;
using CleanArchitecture.Mediator.Internal;
using CleanArchitecture.Mediator.Pipes;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Integration
{

    public class PipelineTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IAuthorisationEnforcer<InputPort, AuthorisationResult>> m_MockAuthEnforcer = new();
        private readonly Mock<IAuthenticatedClaimsPrincipalProvider> m_MockClaimsPrincipalProvider = new();
        private readonly Mock<IEmptyOutputPort> m_MockEmptyOutputPort = new();
        private readonly Mock<IInteractor<InputPort, IEmptyOutputPort>> m_MockEmptyOutputPortInteractor = new();
        private readonly Mock<IEverythingOutputPort> m_MockEverythingOutputPort = new();
        private readonly Mock<IValidator<InputPort, ValidationResult>> m_MockInputPortValidator = new();
        private readonly Mock<ServiceFactory> m_MockServiceFactory = new();

        private readonly AuthorisationResult m_AuthResult = new() { IsAuthorised = true };
        private readonly InputPort m_InputPort = new();
        private readonly Pipeline m_Pipeline;
        private readonly ValidationResult m_ValidationResult = new() { IsValid = true };

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public PipelineTests()
        {
            var _PackageConfiguration = CleanArchitectureMediator.Configure(builder
                => builder.AddPipeline<Pipeline>(pipeline
                    => pipeline
                        .AddAuthentication()
                        .AddAuthorisation<AuthorisationResult>()
                        .AddValidation<ValidationResult>()
                        .AddInteractorInvocation()));

            var _PipelineHandleFactory = new PipelineHandleFactory(this.m_MockServiceFactory.Object);

            this.m_Pipeline = new Pipeline(this.m_MockServiceFactory.Object);

            _ = this.m_MockAuthEnforcer
                    .Setup(mock => mock.CheckAuthorisationAsync(this.m_InputPort, default))
                    .Returns(Task.FromResult(this.m_AuthResult));

            _ = this.m_MockClaimsPrincipalProvider
                    .Setup(mock => mock.AuthenticatedClaimsPrincipal)
                    .Returns(new ClaimsPrincipal());

            _ = this.m_MockInputPortValidator
                    .Setup(mock => mock.ValidateAsync(this.m_InputPort, default))
                    .Returns(Task.FromResult(this.m_ValidationResult));

            _ = this.m_MockServiceFactory
                    .Setup(mock => mock.Invoke(typeof(IEnumerable<IPipe>)))
                    .Returns(new List<IPipe>()
                    {
                        new AuthenticationPipe(),
                        new AuthorisationPipe<AuthorisationResult>(),
                        new ValidationPipe<ValidationResult>(),
                        new InteractorInvocationPipe()
                    });

            _ = this.m_MockServiceFactory
                    .Setup(mock => mock.Invoke(typeof(IPipelineHandleFactory)))
                    .Returns(_PipelineHandleFactory);

            _ = this.m_MockServiceFactory
                    .Setup(mock => mock.Invoke(typeof(IAuthorisationEnforcer<InputPort, AuthorisationResult>)))
                    .Returns(this.m_MockAuthEnforcer.Object);

            _ = this.m_MockServiceFactory
                    .Setup(mock => mock.Invoke(typeof(IAuthenticatedClaimsPrincipalProvider)))
                    .Returns(this.m_MockClaimsPrincipalProvider.Object);

            _ = this.m_MockServiceFactory
                    .Setup(mock => mock.Invoke(typeof(IInteractor<InputPort, IEmptyOutputPort>)))
                    .Returns(this.m_MockEmptyOutputPortInteractor.Object);

            _ = this.m_MockServiceFactory
                    .Setup(mock => mock.Invoke(typeof(IValidator<InputPort, ValidationResult>)))
                    .Returns(this.m_MockInputPortValidator.Object);

            _ = this.m_MockServiceFactory
                    .Setup(mock => mock.Invoke(typeof(PackageConfiguration)))
                    .Returns(_PackageConfiguration);
        }

        #endregion Constructors

        #region - - - - - - InvokeAsync Tests - - - - - -

        [Fact]
        public async Task InvokeAsync_EmptyOutputPort_InvokesInteractor()
        {
            // Arrange

            // Act
            await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEmptyOutputPort.Object, default);

            // Assert
            this.m_MockEmptyOutputPortInteractor.Verify(mock => mock.HandleAsync(this.m_InputPort, this.m_MockEmptyOutputPort.Object, default), Times.Once());
        }

        [Fact]
        public async Task InvokeAsync_EverythingOutputPortWithAuthenticationFailure_StopsWithAuthenticationFailure()
        {
            // Arrange
            this.m_MockClaimsPrincipalProvider.Reset();

            // Act
            await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, default);

            // Assert
            this.m_MockAuthEnforcer.Verify(mock => mock.CheckAuthorisationAsync(this.m_InputPort, default), Times.Never());
            this.m_MockClaimsPrincipalProvider.Verify(mock => mock.AuthenticatedClaimsPrincipal, Times.Once());
            this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, default), Times.Never());

            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentUnauthenticatedAsync(default), Times.Once());
            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentValidationFailureAsync(this.m_ValidationResult, default), Times.Never());
            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentUnauthorisedAsync(this.m_AuthResult, default), Times.Never());
        }

        [Fact]
        public async Task InvokeAsync_EverythingOutputPortWithAuthorisationFailure_StopsWithAuthorisationFailure()
        {
            // Arrange
            this.m_AuthResult.IsAuthorised = false;

            // Act
            await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, default);

            // Assert
            this.m_MockAuthEnforcer.Verify(mock => mock.CheckAuthorisationAsync(this.m_InputPort, default), Times.Once());
            this.m_MockClaimsPrincipalProvider.Verify(mock => mock.AuthenticatedClaimsPrincipal, Times.Once());
            this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, default), Times.Never());

            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentUnauthenticatedAsync(default), Times.Never());
            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentValidationFailureAsync(this.m_ValidationResult, default), Times.Never());
            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentUnauthorisedAsync(this.m_AuthResult, default), Times.Once());
        }

        [Fact]
        public async Task InvokeAsync_EverythingOutputPortWithValidationFailure_StopsWithValidationFailure()
        {
            // Arrange
            this.m_ValidationResult.IsValid = false;

            // Act
            await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, default);

            // Assert
            this.m_MockAuthEnforcer.Verify(mock => mock.CheckAuthorisationAsync(this.m_InputPort, default), Times.Once());
            this.m_MockClaimsPrincipalProvider.Verify(mock => mock.AuthenticatedClaimsPrincipal, Times.Once());
            this.m_MockInputPortValidator.Verify(mock => mock.ValidateAsync(this.m_InputPort, default), Times.Once());

            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentUnauthenticatedAsync(default), Times.Never());
            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentValidationFailureAsync(this.m_ValidationResult, default), Times.Once());
            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentUnauthorisedAsync(this.m_AuthResult, default), Times.Never());
        }

        #endregion InvokeAsync Tests

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
            IValidationOutputPort<ValidationResult>
        { }

        public class InputPort :
            IInputPort<IEmptyOutputPort>,
            IInputPort<IEverythingOutputPort>
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
