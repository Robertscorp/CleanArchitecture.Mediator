using CleanArchitecture.Mediator.Internal;
using Slender.AssemblyScanner;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CleanArchitecture.Mediator.Configuration
{

    /// <summary>
    /// A builder used to configure the CleanArchitecture.Mediator package.
    /// </summary>
    public class PackageConfigurationBuilder
    {

        #region - - - - - - Fields - - - - - -

        private readonly Action<Type, Func<ServiceFactory, object>> m_OnSingletonFactoryAdded;
        private readonly Action<Type, Type> m_OnSingletonServiceAdded;
        private readonly HashSet<Type> m_ServicesToScan = new HashSet<Type>();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        internal PackageConfigurationBuilder(Action<Type, Func<ServiceFactory, object>> onSingletonFactoryAdded, Action<Type, Type> onSingletonServiceAdded)
        {
            this.m_OnSingletonFactoryAdded = onSingletonFactoryAdded ?? throw new ArgumentNullException(nameof(onSingletonFactoryAdded));
            this.m_OnSingletonServiceAdded = onSingletonServiceAdded ?? throw new ArgumentNullException(nameof(onSingletonServiceAdded));
        }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Adds and configures a pipeline.
        /// </summary>
        /// <typeparam name="TPipeline">The type of pipeline to add.</typeparam>
        /// <param name="configurationAction">An action used to configure the pipeline.</param>
        /// <returns>Itself.</returns>
        public PackageConfigurationBuilder AddPipeline<TPipeline>(Action<PipelineBuilder<TPipeline>> configurationAction) where TPipeline : Pipeline
        {
            var _PipelineBuilder = new PipelineBuilder<TPipeline>(type => this.m_OnSingletonServiceAdded(type, type), type => this.m_ServicesToScan.Add(type));

            configurationAction(_PipelineBuilder);

            this.m_OnSingletonFactoryAdded(typeof(PipelineHandleAccessor<TPipeline>), _PipelineBuilder.GetPipelineHandleAccessorFactory());
            this.m_OnSingletonServiceAdded(typeof(TPipeline), typeof(TPipeline));

            return this;
        }

        internal void ScanAssembliesForServiceImplementations(IEnumerable<Assembly> assemblies)
            => new ServiceFinder()
            {
                OnServiceImplementationsFound = (service, implementations) =>
                {
                    if (this.m_ServicesToScan.Contains(service) || (service.IsGenericType && this.m_ServicesToScan.Contains(service.GetGenericTypeDefinition())))
                        foreach (var _Implementation in implementations)
                            this.m_OnSingletonServiceAdded(service, _Implementation);
                }
            }.VisitAssemblyScan(AssemblyScan.FromAssemblies(assemblies));

        #endregion Methods

        #region - - - - - - Nested Classes - - - - - -

        private class ServiceFinder : AssemblyScanVisitor
        {

            #region - - - - - - Properties - - - - - -

            public Action<Type, IEnumerable<Type>> OnServiceImplementationsFound { get; set; }

            #endregion Properties

            #region - - - - - - Methods - - - - - -

            protected override void VisitAbstractAndImplementations(Type abstractType, IEnumerable<Type> implementationTypes)
                => this.OnServiceImplementationsFound(abstractType, implementationTypes);

            protected override void VisitInterfaceAndImplementations(Type interfaceType, IEnumerable<Type> implementationTypes)
                => this.OnServiceImplementationsFound(interfaceType, implementationTypes);

            #endregion Methods

        }

        #endregion Nested Classes

    }

}
