using CleanArchitecture.Services.Authentication;
using CleanArchitecture.Services.Infrastructure;
using CleanArchitecture.Services.Pipeline;

namespace CleanArchitecture.Services.DependencyInjection
{

    public class PipelineOptions
    {

        #region - - - - - - Properties - - - - - -

        internal List<UseCaseElementOptions> ElementOptions { get; } = new();

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        public PipelineOptions AddAuthentication()
            => this.AddUseCaseElement<AuthenticationUseCaseElement>(opts
                => _ = opts.AddUseCaseService<IAuthenticatedClaimsPrincipalProvider>());

        public PipelineOptions AddAuthorisation<TAuthorisationResult>() where TAuthorisationResult : IAuthorisationResult
            => this.AddUseCaseElement<AuthorisationUseCaseElement<TAuthorisationResult>>(opts
                => _ = opts.AddUseCaseService(typeof(IUseCaseAuthorisationEnforcer<,>)));

        public PipelineOptions AddBusinessRuleValidation<TValidationResult>() where TValidationResult : IValidationResult
            => this.AddUseCaseElement<BusinessRuleValidatorUseCaseElement<TValidationResult>>(opts
                => _ = opts.AddUseCaseService(typeof(IUseCaseBusinessRuleValidator<,>)));

        public PipelineOptions AddInputPortValidation<TValidationResult>() where TValidationResult : IValidationResult
            => this.AddUseCaseElement<InputPortValidatorUseCaseElement<TValidationResult>>(opts
                => _ = opts.AddUseCaseService(typeof(IUseCaseInputPortValidator<,>)));

        public PipelineOptions AddInteractorInvocation()
            => this.AddUseCaseElement<InteractorUseCaseElement>(opts
                => _ = opts.AddUseCaseService(typeof(IUseCaseInteractor<,>)));

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
