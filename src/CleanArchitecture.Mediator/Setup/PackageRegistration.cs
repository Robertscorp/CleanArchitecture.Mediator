using Slender.AssemblyScanner;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CleanArchitecture.Mediator.Setup
{

    /// <summary>
    /// Represents registering the CleanArchitecture.Mediator package in the service container.
    /// </summary>
    public class PackageRegistration
    {

        #region - - - - - - Fields - - - - - -

        private readonly HashSet<Assembly> m_AssembliesToScan = new HashSet<Assembly>();
        private readonly HashSet<Type> m_ScopedServicesToScan = new HashSet<Type>();
        private readonly HashSet<Type> m_SingletonServicesToScan = new HashSet<Type>();

        private Action<Type, Type> m_ScopedServiceRegistrationAction
            = (_, __) => throw new InvalidOperationException($"Must specify a scoped service registration action by calling '{nameof(WithScopedServiceRegistrationAction)}' prior to calling '{nameof(AddScopedServiceImplementation)}'.");

        private Action<Type, object> m_SingletonInstanceRegistrationAction
            = (_, __) => throw new InvalidOperationException($"Must specify a singleton instance registration action by calling '{nameof(WithSingletonInstanceRegistrationAction)}' prior to calling '{nameof(AddSingletonInstance)}'.");

        private Action<Type, Type> m_SingletonServiceRegistrationAction
            = (_, __) => throw new InvalidOperationException($"Must specify a singleton service registration action by calling '{nameof(WithSingletonServiceRegistrationAction)}' prior to calling '{nameof(AddSingletonServiceImplementation)}'.");

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        internal PackageRegistration() { }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Adds assemblies to scan for service implementations.
        /// </summary>
        /// <param name="assemblies">The assemblies to scan for service implementations. Cannot be null.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="assemblies"/> is null.</exception>
        public PackageRegistration AddAssemblies(params Assembly[] assemblies)
        {
            if (assemblies is null) throw new ArgumentNullException(nameof(assemblies));

            foreach (var _Assembly in assemblies)
                _ = this.m_AssembliesToScan.Add(_Assembly);

            return this;
        }

        /// <summary>
        /// Adds a scoped service type to find implementations for.
        /// </summary>
        /// <param name="serviceType">The type of service to find implementations for. Cannot be null.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> is null.</exception>
        /// <remarks>When registering generic types, the generic type definition should be provided.</remarks>
        public PackageRegistration AddScopedService(Type serviceType)
        {
            _ = this.m_ScopedServicesToScan.Add(serviceType ?? throw new ArgumentNullException(nameof(serviceType)));
            return this;
        }

        /// <summary>
        /// Adds a scoped service instance to be registered in the service container.
        /// </summary>
        /// <param name="serviceType">The type of service to register. Cannot be null.</param>
        /// <param name="implementationType">The type of implementation to register. Cannot be null.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="implementationType"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> is null.</exception>
        public PackageRegistration AddScopedServiceImplementation(Type serviceType, Type implementationType)
        {
            this.m_ScopedServiceRegistrationAction(
                serviceType ?? throw new ArgumentNullException(nameof(serviceType)),
                implementationType ?? throw new ArgumentNullException(nameof(implementationType)));

            return this;
        }

        /// <summary>
        /// Adds a singleton instance to be registered in the service container.
        /// </summary>
        /// <param name="serviceType">The type of service to register. Cannot be null.</param>
        /// <param name="instance">The singleton instance to register. Cannot be null.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> is null.</exception>
        public PackageRegistration AddSingletonInstance(Type serviceType, object instance)
        {
            this.m_SingletonInstanceRegistrationAction(
                serviceType ?? throw new ArgumentNullException(nameof(serviceType)),
                instance ?? throw new ArgumentNullException(nameof(instance)));

            return this;
        }

        /// <summary>
        /// Adds a singleton service type to find implementations for.
        /// </summary>
        /// <param name="serviceType">The type of service to find implementations for. Cannot be null.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> is null.</exception>
        /// <remarks>When registering generic types, the generic type definition should be provided.</remarks>
        public PackageRegistration AddSingletonService(Type serviceType)
        {
            _ = this.m_SingletonServicesToScan.Add(serviceType ?? throw new ArgumentNullException(nameof(serviceType)));
            return this;
        }

        /// <summary>
        /// Adds a singleton service instance to be registered in the service container.
        /// </summary>
        /// <param name="serviceType">The type of service to register. Cannot be null.</param>
        /// <param name="implementationType">The type of implementation to register. Cannot be null.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="implementationType"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> is null.</exception>
        public PackageRegistration AddSingletonServiceImplementation(Type serviceType, Type implementationType)
        {
            this.m_SingletonServiceRegistrationAction(
                serviceType ?? throw new ArgumentNullException(nameof(serviceType)),
                implementationType ?? throw new ArgumentNullException(nameof(implementationType)));

            return this;
        }

        /// <summary>
        /// Defines how to register a scoped service in the service container.
        /// </summary>
        /// <param name="scopedServiceRegistrationAction">The action to register the specified serviceType and implementationType in the service container as a scoped service. Cannot be null.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="scopedServiceRegistrationAction"/> is null.</exception>
        public PackageRegistration WithScopedServiceRegistrationAction(Action<Type, Type> scopedServiceRegistrationAction)
        {
            this.m_ScopedServiceRegistrationAction = scopedServiceRegistrationAction ?? throw new ArgumentNullException(nameof(scopedServiceRegistrationAction));
            return this;
        }

        /// <summary>
        /// Defines how to register a singleton instance in the service container.
        /// </summary>
        /// <param name="singletonInstanceRegistrationAction">The action to register the specified serviceType and instance in the service container as a singleton. Cannot be null.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="singletonInstanceRegistrationAction"/> is null.</exception>
        public PackageRegistration WithSingletonInstanceRegistrationAction(Action<Type, object> singletonInstanceRegistrationAction)
        {
            this.m_SingletonInstanceRegistrationAction = singletonInstanceRegistrationAction ?? throw new ArgumentNullException(nameof(singletonInstanceRegistrationAction));
            return this;
        }

        /// <summary>
        /// Defines how to register a singleton service in the service container.
        /// </summary>
        /// <param name="singletonServiceRegistrationAction">The action to register the specified serviceType and implementationType in the service container as a singleton service. Cannot be null.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="singletonServiceRegistrationAction"/> is null.</exception>
        public PackageRegistration WithSingletonServiceRegistrationAction(Action<Type, Type> singletonServiceRegistrationAction)
        {
            this.m_SingletonServiceRegistrationAction = singletonServiceRegistrationAction ?? throw new ArgumentNullException(nameof(singletonServiceRegistrationAction));
            return this;
        }

        internal void ScanForImplementations()
            => new ServiceFinder()
            {
                OnServiceImplementationsFound = (service, implementations) =>
                {
                    if (this.m_SingletonServicesToScan.Contains(service) || service.IsGenericType && this.m_SingletonServicesToScan.Contains(service.GetGenericTypeDefinition()))
                        foreach (var _Implementation in implementations)
                            this.m_SingletonServiceRegistrationAction(service, _Implementation);

                    else if (this.m_ScopedServicesToScan.Contains(service) || service.IsGenericType && this.m_ScopedServicesToScan.Contains(service.GetGenericTypeDefinition()))
                        foreach (var _Implementation in implementations)
                            this.m_ScopedServiceRegistrationAction(service, _Implementation);
                }
            }.VisitAssemblyScan(AssemblyScan.FromAssemblies(this.m_AssembliesToScan));

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
