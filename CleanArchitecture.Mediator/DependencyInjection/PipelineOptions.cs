using CleanArchitecture.Mediator.Authentication;
using CleanArchitecture.Mediator.Infrastructure;
using CleanArchitecture.Mediator.Pipeline;
using System;
using System.Collections.Generic;

namespace CleanArchitecture.Mediator.DependencyInjection
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

        internal List<PipeOptions> PipeOptions { get; } = new List<PipeOptions>();

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Registers authentication as the next Pipe in the Use Case Pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        public PipelineOptions AddAuthentication()
            => this.AddUseCasePipe<AuthenticationPipe>(opts
                => opts
                    .AddPipeService<IAuthenticatedClaimsPrincipalProvider>()
                    .WithPipeOutputPort<IAuthenticationOutputPort>());

        /// <summary>
        /// Registers authorisation as the next Pipe in the Use Case Pipeline.
        /// </summary>
        /// <typeparam name="TAuthorisationResult">The type of authorisation result used in the Use Case Pipeline.</typeparam>
        /// <returns>Itself.</returns>
        public PipelineOptions AddAuthorisation<TAuthorisationResult>() where TAuthorisationResult : IAuthorisationResult
            => this.AddUseCasePipe<AuthorisationPipe<TAuthorisationResult>>(opts
                => opts
                    .AddPipeService(typeof(IUseCaseAuthorisationEnforcer<,>))
                        .WithUseCaseServiceResolver((useCaseInputPort, useCaseOutputPort, pipeService)
                            => pipeService.MakeGenericType(useCaseInputPort, typeof(TAuthorisationResult)))
                    .WithPipeOutputPort<IAuthorisationOutputPort<TAuthorisationResult>>());

        /// <summary>
        /// Registers business rule validation as the next Pipe in the Use Case Pipeline.
        /// </summary>
        /// <typeparam name="TValidationResult">The type of validation result used in the Use Case Pipeline.</typeparam>
        /// <returns>Itself.</returns>
        public PipelineOptions AddBusinessRuleValidation<TValidationResult>() where TValidationResult : IValidationResult
            => this.AddUseCasePipe<BusinessRuleValidationPipe<TValidationResult>>(opts
                => opts
                    .AddPipeService(typeof(IUseCaseBusinessRuleValidator<,>))
                        .WithUseCaseServiceResolver((useCaseInputPort, useCaseOutputPort, pipeService)
                            => pipeService.MakeGenericType(useCaseInputPort, typeof(TValidationResult)))
                    .WithPipeOutputPort<IBusinessRuleValidationOutputPort<TValidationResult>>());

        /// <summary>
        /// Registers Input Port validation as the next Pipe in the Use Case Pipeline.
        /// </summary>
        /// <typeparam name="TValidationResult">The type of validation result used in the Use Case Pipeline.</typeparam>
        /// <returns>Itself.</returns>
        public PipelineOptions AddInputPortValidation<TValidationResult>() where TValidationResult : IValidationResult
            => this.AddUseCasePipe<InputPortValidationPipe<TValidationResult>>(opts
                => opts
                    .AddPipeService(typeof(IUseCaseInputPortValidator<,>))
                        .WithUseCaseServiceResolver((useCaseInputPort, useCaseOutputPort, pipeService)
                            => pipeService.MakeGenericType(useCaseInputPort, typeof(TValidationResult)))
                    .WithPipeOutputPort<IValidationOutputPort<TValidationResult>>());

        /// <summary>
        /// Registers interactor invocation as the final Pipe in the Use Case Pipeline.
        /// </summary>
        /// <remarks>
        /// This Pipe is a terminal point of the Use Case Pipeline.
        /// This means that any Pipe registered after this will never be invoked.
        /// </remarks>
        public void AddInteractorInvocation()
            => this.AddUseCasePipe<InteractorInvocationPipe>(opts
                => _ = opts.AddPipeService(typeof(IUseCaseInteractor<,>)));

        /// <summary>
        /// Registers custom behaviour as the next Pipe in the Use Case Pipeline.
        /// </summary>
        /// <typeparam name="TUseCasePipe">The type of Pipe to register.</typeparam>
        /// <param name="configurationAction">The action to configure the Pipe.</param>
        /// <returns>Itself.</returns>
        public PipelineOptions AddUseCasePipe<TUseCasePipe>(Action<PipeOptions> configurationAction) where TUseCasePipe : IUseCasePipe
        {
            var _Options = new PipeOptions(typeof(TUseCasePipe));
            configurationAction(_Options);
            this.PipeOptions.Add(_Options);
            return this;
        }

        #endregion Methods

    }

}
