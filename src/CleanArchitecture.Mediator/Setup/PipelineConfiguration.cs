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
        private readonly List<Func<ServiceFactory, IPipeHandle, IPipeHandle>> m_PipeHandleProviders = new List<Func<ServiceFactory, IPipeHandle, IPipeHandle>>();

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
        /// or if <see cref="IPrincipalAccessor"/> is going to be manually registered.
        /// </remarks>
        public PipelineConfiguration<TPipeline> AddAuthentication()
            => this.AddPipe<AuthenticationPipe>();

        /// <summary>
        /// Adds authorisation policy validation to the pipeline.
        /// </summary>
        /// <typeparam name="TPolicyFailure">The type of authorisation policy failure.</typeparam>
        /// <returns>Itself.</returns>
        public PipelineConfiguration<TPipeline> AddAuthorisationPolicyValidation<TPolicyFailure>()
            => this.AddOpenGenericPipe(
                typeof(AuthorisationPolicyValidationPipe<,,>),
                new Type[] { typeof(TPolicyFailure) },
                config => config.AddSingletonService(typeof(IAuthorisationPolicyValidator<,>)));

        /// <summary>
        /// Adds business rule evaluation to the pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        public PipelineConfiguration<TPipeline> AddBusinessRuleEvaluation()
            => this.AddPipe<BusinessRuleEvaluationPipe>(config => config.AddSingletonService(typeof(IBusinessRuleEvaluator<,>)));

        /// <summary>
        /// Adds input port validation to the pipeline.
        /// </summary>
        /// <typeparam name="TValidationFailure">The type of input port validation failure.</typeparam>
        /// <returns>Itself.</returns>
        public PipelineConfiguration<TPipeline> AddInputPortValidation<TValidationFailure>()
            => this.AddOpenGenericPipe(
                typeof(InputPortValidationPipe<,,>),
                new Type[] { typeof(TValidationFailure) },
                config => config.AddSingletonService(typeof(IInputPortValidator<,>)));

        /// <summary>
        /// Adds interactor invocation to the pipeline.
        /// </summary>
        public void AddInteractorInvocation()
            => this.AddPipe<InteractorInvocationPipe>(config => config.AddSingletonService(typeof(IInteractor<,>)));

        /// <summary>
        /// Adds licence policy validation to the pipeline.
        /// </summary>
        /// <typeparam name="TPolicyFailure">The type of licence policy failure.</typeparam>
        /// <returns>Itself.</returns>
        public PipelineConfiguration<TPipeline> AddLicencePolicyValidation<TPolicyFailure>()
            => this.AddOpenGenericPipe(
                typeof(LicencePolicyValidationPipe<,,>),
                new Type[] { typeof(TPolicyFailure) },
                config => config.AddSingletonService(typeof(ILicencePolicyValidator<,>)));

        /// <summary>
        /// Adds a pipe to the pipeline.
        /// </summary>
        /// <typeparam name="TPipe">The type of pipe to add to the pipeline.</typeparam>
        /// <param name="registrationConfigurationAction">The action to register the services that are produced by the <see cref="ServiceFactory"/> within the <typeparamref name="TPipe"/>.</param>
        /// <returns>Itself.</returns>
        public PipelineConfiguration<TPipeline> AddPipe<TPipe>(Action<PackageRegistration> registrationConfigurationAction = null) where TPipe : IPipe
        {
            this.m_PipeHandleProviders.Add((serviceFactory, nextPipeHandle) => new NonGenericPipeHandle(serviceFactory.GetService<TPipe>(), nextPipeHandle));
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

            this.m_PipeHandleProviders.Add((_, nextPipeHandle) => new InlinePipe(inlineBehaviourAsync, nextPipeHandle));
            registrationConfigurationAction?.Invoke(this.m_PackageRegistration);

            return this;
        }

        /// <summary>
        /// Adds multi-tenant authentication to the pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        /// <remarks>Should not be used in conjunction with single-tenant authentication.</remarks>
        public PipelineConfiguration<TPipeline> AddMultiTenantAuthentication()
            => this.AddPipe<AuthenticationPipe>(config => config.AddScopedService(typeof(IPrincipalAccessor)));

        /// <summary>
        /// Adds an open generic pipe to the pipeline.
        /// </summary>
        /// <param name="openGenericPipeType">The <see cref="Type"/> of open generic pipe. Cannot be null.</param>
        /// <param name="closedGenericTypes">The types (excluding input port and output port) required to close the open generic pipe. Cannot be null.</param>
        /// <param name="registrationConfigurationAction">The action to register the services that are produced by the <see cref="ServiceFactory"/> within the pipe.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentException"><paramref name="openGenericPipeType"/> is not an open generic type.</exception>
        /// <exception cref="ArgumentException"><paramref name="openGenericPipeType"/> does not implement <see cref="IPipe{TInputPort, TOutputPort}"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="openGenericPipeType"/> implements <see cref="IPipe{TInputPort, TOutputPort}"/> more than once.</exception>
        /// <exception cref="ArgumentException">The TInputPort parameter of the <see cref="IPipe{TInputPort, TOutputPort}"/> implementation is not a generic parameter.</exception>
        /// <exception cref="ArgumentException">The TOutputPort parameter of the <see cref="IPipe{TInputPort, TOutputPort}"/> implementation is not a generic parameter.</exception>
        /// <exception cref="ArgumentException">An incorrect number of closed generic parameters are specified in the <paramref name="closedGenericTypes"/> collection.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="closedGenericTypes"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="openGenericPipeType"/> is null.</exception>
        public PipelineConfiguration<TPipeline> AddOpenGenericPipe(Type openGenericPipeType, Type[] closedGenericTypes, Action<PackageRegistration> registrationConfigurationAction = null)
        {
            if (openGenericPipeType is null) throw new ArgumentNullException(nameof(openGenericPipeType));
            if (closedGenericTypes is null) throw new ArgumentNullException(nameof(closedGenericTypes));

            // This is declared outside the PipHandleProvider function to ensure the checks are done during registration.
            var _PipeProvider = new ClosedGenericPipeProvider(openGenericPipeType, closedGenericTypes);

            this.m_PipeHandleProviders.Add((_, nextPipeHandle) => new OpenGenericPipeHandle(_PipeProvider, nextPipeHandle));
            _ = this.m_PackageRegistration.AddSingletonServiceImplementation(openGenericPipeType, openGenericPipeType);
            registrationConfigurationAction?.Invoke(this.m_PackageRegistration);

            return this;
        }

        /// <summary>
        /// Adds single-tenant authentication to the pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        /// <remarks>
        /// Should not be used in conjunction with multi-tenant authentication.
        /// </remarks>
        public PipelineConfiguration<TPipeline> AddSingleTenantAuthentication()
            => this.AddPipe<AuthenticationPipe>(config => config.AddSingletonService(typeof(IPrincipalAccessor)));

        internal Func<ServiceFactory, PipelineHandleAccessor<TPipeline>> GetPipelineHandleAccessorFactory()
            => serviceFactory
                => new PipelineHandleAccessor<TPipeline>(
                    this.m_PipeHandleProviders
                        .AsEnumerable()
                        .Reverse()
                        .Aggregate(
                            (IPipeHandle)new TerminalPipeHandle(),
                            (nextPipeHandle, pipeHandleProvider) => pipeHandleProvider(serviceFactory, nextPipeHandle)));

        #endregion Methods

    }

}
