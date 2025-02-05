using CleanArchitecture.Mediator.Configuration;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Integration
{

    public class PipelineTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IAuthorisationEnforcer<InputPort, IEverythingOutputPort>> m_MockAuthEnforcer = new();
        private readonly Mock<IAuthenticatedClaimsPrincipalProvider> m_MockClaimsPrincipalProvider = new();
        private readonly Mock<IEmptyOutputPort> m_MockEmptyOutputPort = new();
        private readonly Mock<IInteractor<InputPort, IEmptyOutputPort>> m_MockEmptyOutputPortInteractor = new();
        private readonly Mock<IEverythingOutputPort> m_MockEverythingOutputPort = new();
        private readonly Mock<ServiceFactory> m_MockServiceFactory = new();
        private readonly Mock<IValidator<InputPort, IEverythingOutputPort>> m_MockValidator = new();

        private readonly InputPort m_InputPort = new();
        private readonly Pipeline m_Pipeline;

        private bool m_AuthResult = true;
        private bool m_ValidationResult = true;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public PipelineTests()
        {
            _ = this.m_MockAuthEnforcer
                    .Setup(mock => mock.HandleAuthorisationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, default))
                    .Returns(() => Task.FromResult(this.m_AuthResult));

            _ = this.m_MockClaimsPrincipalProvider
                    .Setup(mock => mock.AuthenticatedClaimsPrincipal)
                    .Returns(new ClaimsPrincipal());

            _ = this.m_MockValidator
                    .Setup(mock => mock.HandleValidationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, default))
                    .Returns(() => Task.FromResult(this.m_ValidationResult));

            _ = this.m_MockServiceFactory
                    .Setup(mock => mock.Invoke(typeof(IAuthorisationEnforcer<InputPort, IEverythingOutputPort>)))
                    .Returns(this.m_MockAuthEnforcer.Object);

            _ = this.m_MockServiceFactory
                    .Setup(mock => mock.Invoke(typeof(IAuthenticatedClaimsPrincipalProvider)))
                    .Returns(this.m_MockClaimsPrincipalProvider.Object);

            _ = this.m_MockServiceFactory
                    .Setup(mock => mock.Invoke(typeof(IInteractor<InputPort, IEmptyOutputPort>)))
                    .Returns(this.m_MockEmptyOutputPortInteractor.Object);

            _ = this.m_MockServiceFactory
                    .Setup(mock => mock.Invoke(typeof(IValidator<InputPort, IEverythingOutputPort>)))
                    .Returns(this.m_MockValidator.Object);

            CleanArchitectureMediator.Configure(builder
                => builder.AddPipeline<Pipeline>(pipeline
                    => pipeline
                        .AddAuthentication()
                        .AddAuthorisation()
                        .AddValidation()
                        .AddInteractorInvocation()),
                        type => this.m_MockServiceFactory.Setup(mock => mock.Invoke(type)).Returns((Type t) => Activator.CreateInstance(t)!),
                        (type, factory) => this.m_MockServiceFactory.Setup(mock => mock.Invoke(type)).Returns(factory(this.m_MockServiceFactory.Object)));


            this.m_Pipeline = new Pipeline(this.m_MockServiceFactory.Object);
        }

        #endregion Constructors

        #region - - - - - - InvokeAsync Tests - - - - - -

        [Fact]
        public async Task InvokeAsync_EmptyOutputPort_InvokesInteractor()
        {
            // Arrange

            // Act
            await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEmptyOutputPort.Object, this.m_MockServiceFactory.Object, default);

            // Assert
            this.m_MockEmptyOutputPortInteractor.Verify(mock => mock.HandleAsync(this.m_InputPort, this.m_MockEmptyOutputPort.Object, default), Times.Once());
        }

        [Fact]
        public async Task InvokeAsync_EverythingOutputPortWithAuthenticationFailure_StopsWithAuthenticationFailure()
        {
            // Arrange
            this.m_MockClaimsPrincipalProvider.Reset();

            // Act
            await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default);

            // Assert
            this.m_MockAuthEnforcer.Verify(mock => mock.HandleAuthorisationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, default), Times.Never());
            this.m_MockClaimsPrincipalProvider.Verify(mock => mock.AuthenticatedClaimsPrincipal, Times.Once());
            this.m_MockValidator.Verify(mock => mock.HandleValidationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, default), Times.Never());

            this.m_MockEverythingOutputPort.Verify(mock => mock.PresentUnauthenticatedAsync(default), Times.Once());
            this.m_MockEverythingOutputPort.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task InvokeAsync_EverythingOutputPortWithAuthorisationFailure_StopsWithAuthorisationFailure()
        {
            // Arrange
            this.m_AuthResult = false;

            // Act
            await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default);

            // Assert
            this.m_MockAuthEnforcer.Verify(mock => mock.HandleAuthorisationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, default), Times.Once());
            this.m_MockClaimsPrincipalProvider.Verify(mock => mock.AuthenticatedClaimsPrincipal, Times.Once());
            this.m_MockValidator.Verify(mock => mock.HandleValidationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, default), Times.Never());

            this.m_MockEverythingOutputPort.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task InvokeAsync_EverythingOutputPortWithValidationFailure_StopsWithValidationFailure()
        {
            // Arrange
            this.m_ValidationResult = false;

            // Act
            await this.m_Pipeline.InvokeAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, this.m_MockServiceFactory.Object, default);

            // Assert
            this.m_MockAuthEnforcer.Verify(mock => mock.HandleAuthorisationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, default), Times.Once());
            this.m_MockClaimsPrincipalProvider.Verify(mock => mock.AuthenticatedClaimsPrincipal, Times.Once());
            this.m_MockValidator.Verify(mock => mock.HandleValidationAsync(this.m_InputPort, this.m_MockEverythingOutputPort.Object, default), Times.Once());

            this.m_MockEverythingOutputPort.VerifyNoOtherCalls();
        }

        #endregion InvokeAsync Tests

        #region - - - - - - Nested Classes - - - - - -

        public interface IEmptyOutputPort { }

        public interface IEverythingOutputPort : IAuthenticationOutputPort { }

        public class InputPort : IInputPort<IEmptyOutputPort>, IInputPort<IEverythingOutputPort> { }

        #endregion Nested Classes

    }

}
