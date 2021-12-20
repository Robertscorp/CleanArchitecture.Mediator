using CleanArchitecture.Services.Authentication;
using CleanArchitecture.Services.Infrastructure;
using CleanArchitecture.Services.Pipeline;

namespace CleanArchitecture.Services.DependencyInjection
{

    /// <summary>
    /// The options used to configure the Use Case Pipeline.
    /// </summary>
    public class PipelineOptions
    {

        #region - - - - - - Constructors - - - - - -

        internal PipelineOptions() { }

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        internal List<UseCaseElementOptions> ElementOptions { get; } = new();

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Registers authentication as the next pipe in the Use Case Pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        public PipelineOptions AddAuthentication()
            => this.AddUseCaseElement<AuthenticationUseCaseElement>(opts
                => _ = opts.AddUseCaseService<IAuthenticatedClaimsPrincipalProvider>());

        /// <summary>
        /// Registers authorisation as the next pipe in the Use Case Pipeline.
        /// </summary>
        /// <typeparam name="TAuthorisationResult">The type of authorisation result used in the Use Case Pipeline.</typeparam>
        /// <returns>Itself.</returns>
        public PipelineOptions AddAuthorisation<TAuthorisationResult>() where TAuthorisationResult : IAuthorisationResult
            => this.AddUseCaseElement<AuthorisationUseCaseElement<TAuthorisationResult>>(opts
                => _ = opts.AddUseCaseService(typeof(IUseCaseAuthorisationEnforcer<,>)));

        /// <summary>
        /// Registers business rule validation as the next pipe in the Use Case Pipeline.
        /// </summary>
        /// <typeparam name="TValidationResult">The type of validation result used in the Use Case Pipeline.</typeparam>
        /// <returns>Itself.</returns>
        public PipelineOptions AddBusinessRuleValidation<TValidationResult>() where TValidationResult : IValidationResult
            => this.AddUseCaseElement<BusinessRuleValidatorUseCaseElement<TValidationResult>>(opts
                => _ = opts.AddUseCaseService(typeof(IUseCaseBusinessRuleValidator<,>)));

        /// <summary>
        /// Registers Input Port validation as the next pipe in the Use Case Pipeline.
        /// </summary>
        /// <typeparam name="TValidationResult">The type of validation result used in the Use Case Pipeline.</typeparam>
        /// <returns>Itself.</returns>
        public PipelineOptions AddInputPortValidation<TValidationResult>() where TValidationResult : IValidationResult
            => this.AddUseCaseElement<InputPortValidatorUseCaseElement<TValidationResult>>(opts
                => _ = opts.AddUseCaseService(typeof(IUseCaseInputPortValidator<,>)));

        /// <summary>
        /// Registers interactor invocation as the final pipe in the Use Case Pipeline.
        /// </summary>
        /// <remarks>
        /// This pipe is a terminal point of the Use Case Pipeline.
        /// This means that any pipe registered after this will never be invoked.
        /// </remarks>
        public void AddInteractorInvocation()
            => this.AddUseCaseElement<InteractorUseCaseElement>(opts
                => _ = opts.AddUseCaseService(typeof(IUseCaseInteractor<,>)));

        /// <summary>
        /// Registers custom behaviour as the next pipe in the Use Case Pipeline.
        /// </summary>
        /// <typeparam name="TUseCaseElement">The type of pipe to register.</typeparam>
        /// <param name="configurationAction">The action to configure the pipe.</param>
        /// <returns>Itself.</returns>
        public PipelineOptions AddUseCaseElement<TUseCaseElement>(Action<UseCaseElementOptions> configurationAction) where TUseCaseElement : IUseCaseElement
        {
            var _Options = new UseCaseElementOptions(typeof(TUseCaseElement));
            configurationAction(_Options);
            this.ElementOptions.Add(_Options);
            return this;
        }

        #endregion Methods

    }

}
