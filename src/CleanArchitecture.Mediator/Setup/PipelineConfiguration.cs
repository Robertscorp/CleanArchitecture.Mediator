using CleanArchitecture.Mediator.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Setup
{

    /// <summary>
    /// Represents configuring a pipeline.
    /// </summary>
    public class PipelineConfiguration<TPipeline> where TPipeline : Pipeline
    {

        #region - - - - - - Fields - - - - - -

        private readonly PackageRegistration m_PackageRegistration;
        private readonly List<object> m_RegisteredPipes = new List<object>();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        internal PipelineConfiguration(PackageRegistration packageRegistration)
            => this.m_PackageRegistration = packageRegistration;

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Adds authentication to the pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        /// <remarks>
        /// This should only be used if a previously configured pipeline has already added either single-tenant or multi-tenant authentication,
        /// or if <see cref="IAuthenticatedClaimsPrincipalProvider"/> is going to be manually registered.
        /// </remarks>
        public PipelineConfiguration<TPipeline> AddAuthentication()
            => this.AddPipe<AuthenticationPipe>();

        /// <summary>
        /// Adds authorisation to the pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        public PipelineConfiguration<TPipeline> AddAuthorisation()
            => this.AddPipe<AuthorisationPipe>(config => config.AddSingletonService(typeof(IAuthorisationEnforcer<,>)));

        /// <summary>
        /// Adds interactor invocation to the pipeline.
        /// </summary>
        public void AddInteractorInvocation()
            => this.AddPipe<InteractorInvocationPipe>(config => config.AddSingletonService(typeof(IInteractor<,>)));

        /// <summary>
        /// Adds a pipe to the pipeline.
        /// </summary>
        /// <typeparam name="TPipe">The type of pipe to add to the pipeline.</typeparam>
        /// <param name="registrationConfigurationAction">The action to register the services that are produced by the <see cref="ServiceFactory"/> within the <typeparamref name="TPipe"/>.</param>
        /// <returns>Itself.</returns>
        public PipelineConfiguration<TPipeline> AddPipe<TPipe>(Action<PackageRegistration> registrationConfigurationAction = null) where TPipe : IPipe
        {
            this.m_RegisteredPipes.Add(typeof(TPipe));
            _ = this.m_PackageRegistration.AddSingletonServiceImplementation(typeof(TPipe), typeof(TPipe));
            registrationConfigurationAction?.Invoke(this.m_PackageRegistration);

            return this;
        }

        /// <summary>
        /// Adds inline behaviour to the pipeline.
        /// </summary>
        /// <param name="inlineBehaviourAsync">The behaviour of the pipe. The parameters are (inputPort, outputPort, serviceFactory, nextPipeHandleAsync, cancellationToken). Cannot be null.</param>
        /// <param name="registrationConfigurationAction">The action to register the services that are produced by the <see cref="ServiceFactory"/> within the specified <paramref name="inlineBehaviourAsync"/>.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="inlineBehaviourAsync"/> is null.</exception>
        public PipelineConfiguration<TPipeline> AddPipe(
            Func<object, object, ServiceFactory, NextPipeHandleAsync, CancellationToken, Task> inlineBehaviourAsync,
            Action<PackageRegistration> registrationConfigurationAction = null)
        {
            if (inlineBehaviourAsync is null) throw new ArgumentNullException(nameof(inlineBehaviourAsync));

            this.m_RegisteredPipes.Add(new InlinePipe(inlineBehaviourAsync));
            registrationConfigurationAction?.Invoke(this.m_PackageRegistration);

            return this;
        }

        /// <summary>
        /// Adds multi-tenant authentication to the pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        /// <remarks>Should not be used in conjunction with single-tenant authentication.</remarks>
        public PipelineConfiguration<TPipeline> AddMultiTenantAuthentication()
            => this.AddPipe<AuthenticationPipe>(config => config.AddScopedService(typeof(IAuthenticatedClaimsPrincipalProvider)));

        /// <summary>
        /// Adds single-tenant authentication to the pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        /// <remarks>
        /// Should not be used in conjunction with multi-tenant authentication.
        /// </remarks>
        public PipelineConfiguration<TPipeline> AddSingleTenantAuthentication()
            => this.AddPipe<AuthenticationPipe>(config => config.AddSingletonService(typeof(IAuthenticatedClaimsPrincipalProvider)));

        /// <summary>
        /// Adds validation to the pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        public PipelineConfiguration<TPipeline> AddValidation()
            => this.AddPipe<ValidationPipe>(config => config.AddSingletonService(typeof(IValidator<,>)));

        internal Func<ServiceFactory, PipelineHandleAccessor<TPipeline>> GetPipelineHandleAccessorFactory()
            => serviceFactory
                => new PipelineHandleAccessor<TPipeline>(
                    this.m_RegisteredPipes
                        .Select(p => p as IPipe ?? (IPipe)serviceFactory((Type)p))
                        .Reverse()
                        .Aggregate(
                            new PipeHandle(null, null),
                            (nextPipeHandle, pipe) => new PipeHandle(pipe, nextPipeHandle)));

        #endregion Methods

    }

}
