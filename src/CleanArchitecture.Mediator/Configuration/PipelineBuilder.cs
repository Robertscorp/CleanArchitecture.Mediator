using CleanArchitecture.Mediator.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Configuration
{

    /// <summary>
    /// A builder used to configure a pipeline.
    /// </summary>
    public class PipelineBuilder<TPipeline> where TPipeline : Pipeline
    {

        #region - - - - - - Fields - - - - - -

        private readonly Action<Type> m_OnPipeAdded;
        private readonly Action<Type> m_OnServiceAdded;
        private readonly List<Func<ServiceFactory, IPipeHandle, IPipeHandle>> m_PipeHandleProviders = new List<Func<ServiceFactory, IPipeHandle, IPipeHandle>>();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        internal PipelineBuilder(Action<Type> onPipeAdded, Action<Type> onServiceAdded)
        {
            this.m_OnPipeAdded = onPipeAdded ?? throw new ArgumentNullException(nameof(onPipeAdded));
            this.m_OnServiceAdded = onServiceAdded ?? throw new ArgumentNullException(nameof(onServiceAdded));
        }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Adds authentication to the pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        public PipelineBuilder<TPipeline> AddAuthentication()
            => this.AddPipe<AuthenticationPipe>(typeof(IAuthenticatedClaimsPrincipalProvider));

        /// <summary>
        /// Adds authorisation to the pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        public PipelineBuilder<TPipeline> AddAuthorisation()
            => this.AddPipe<AuthorisationPipe>(typeof(IAuthorisationEnforcer<,>));

        /// <summary>
        /// Adds interactor invocation to the pipeline.
        /// </summary>
        public void AddInteractorInvocation()
            => this.AddPipe<InteractorInvocationPipe>(typeof(IInteractor<,>));

        /// <summary>
        /// Adds a pipe to the pipeline.
        /// </summary>
        /// <typeparam name="TPipe">The type of pipe to add to the pipeline.</typeparam>
        /// <param name="serviceTypes">The services that are produced by the <see cref="ServiceFactory"/> within the <typeparamref name="TPipe"/>. For generic types, the generic type definition should be provided.</param>
        /// <returns>Itself.</returns>
        /// <remarks>The specified <paramref name="serviceTypes"/> will be registered as singleton services.</remarks>
        public PipelineBuilder<TPipeline> AddPipe<TPipe>(params Type[] serviceTypes) where TPipe : IPipe
        {
            if (serviceTypes is null) throw new ArgumentNullException(nameof(serviceTypes));

            this.m_OnPipeAdded(typeof(TPipe));
            this.m_PipeHandleProviders.Add((serviceFactory, nextPipeHandle) => new NonGenericPipeHandle(serviceFactory.GetService<TPipe>(), nextPipeHandle));

            foreach (var _ServiceType in serviceTypes)
                this.m_OnServiceAdded(_ServiceType);

            return this;
        }

        /// <summary>
        /// Adds an open generic pipe to the pipeline.
        /// </summary>
        /// <param name="openGenericPipeType">The <see cref="Type"/> of open generic pipe.</param>
        /// <param name="closedGenericTypes">The types (excluding input port and output port) required to close the open generic pipe.</param>
        /// <param name="serviceTypes">The services that are produced by the <see cref="ServiceFactory"/> within the pipe. For generic types, the generic type definition should be provided.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentException"><paramref name="openGenericPipeType"/> is not an open generic type.</exception>
        /// <exception cref="ArgumentException"><paramref name="openGenericPipeType"/> does not implement <see cref="IPipe{TInputPort, TOutputPort}"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="openGenericPipeType"/> implements <see cref="IPipe{TInputPort, TOutputPort}"/> more than once.</exception>
        /// <exception cref="ArgumentException">The TInputPort parameter of the <see cref="IPipe{TInputPort, TOutputPort}"/> implementation is not a generic parameter.</exception>
        /// <exception cref="ArgumentException">The TOutputPort parameter of the <see cref="IPipe{TInputPort, TOutputPort}"/> implementation is not a generic parameter.</exception>
        /// <exception cref="ArgumentException">An incorrect number of closed generic parameters are specified in the <paramref name="closedGenericTypes"/> collection.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="openGenericPipeType"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="closedGenericTypes"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="serviceTypes"/> is null.</exception>
        public PipelineBuilder<TPipeline> AddOpenGenericPipe(Type openGenericPipeType, Type[] closedGenericTypes, params Type[] serviceTypes)
        {
            if (openGenericPipeType is null) throw new ArgumentNullException(nameof(openGenericPipeType));
            if (closedGenericTypes is null) throw new ArgumentNullException(nameof(closedGenericTypes));
            if (serviceTypes is null) throw new ArgumentNullException(nameof(serviceTypes));

            // This is declared outside the PipHandleProvider function to ensure the checks are done during registration.
            var _PipeProvider = new ClosedGenericPipeProvider(openGenericPipeType, closedGenericTypes);

            this.m_PipeHandleProviders.Add((_, nextPipeHandle) => new OpenGenericPipeHandle(_PipeProvider, nextPipeHandle));
            this.m_OnPipeAdded(openGenericPipeType);

            foreach (var _ServiceType in serviceTypes)
                this.m_OnServiceAdded(_ServiceType);

            return this;
        }

        /// <summary>
        /// Adds inline behaviour to the pipeline.
        /// </summary>
        /// <param name="inlineBehaviourAsync">The behaviour of the pipe. The parameters are (inputPort, outputPort, serviceFactory, nextPipeHandleAsync, cancellationToken).</param>
        /// <returns>Itself.</returns>
        public PipelineBuilder<TPipeline> AddPipe(
            Func<object, object, ServiceFactory, NextPipeHandleAsync, CancellationToken, Task> inlineBehaviourAsync)
        {
            if (inlineBehaviourAsync is null) throw new ArgumentNullException(nameof(inlineBehaviourAsync));

            this.m_PipeHandleProviders.Add((_, nextPipeHandle) => new InlinePipe(inlineBehaviourAsync, nextPipeHandle));

            return this;
        }

        /// <summary>
        /// Adds validation to the pipeline.
        /// </summary>
        /// <returns>Itself.</returns>
        public PipelineBuilder<TPipeline> AddValidation()
            => this.AddPipe<ValidationPipe>(typeof(IValidator<,>));

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
