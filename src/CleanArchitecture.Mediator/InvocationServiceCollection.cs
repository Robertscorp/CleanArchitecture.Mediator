using System;
using System.Collections.Generic;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Allows defining specific service instances to be produced when invoking a <see cref="Pipeline"/>.
    /// </summary>
    public class InvocationServiceCollection
    {

        #region - - - - - - Fields - - - - - -

        private readonly ServiceFactory m_ServiceFactory;
        private readonly Dictionary<Type, object> m_ServicesByType = new Dictionary<Type, object>();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        internal InvocationServiceCollection(ServiceFactory serviceFactory)
            => this.m_ServiceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        private object GetService(Type type)
            => this.m_ServicesByType.TryGetValue(type, out var _Service)
                ? _Service
                : this.m_ServiceFactory(type);

        /// <summary>
        /// Uses the specified <paramref name="implementationInstance"/> when producing an instance of <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service to produce an instance for.</typeparam>
        /// <param name="implementationInstance">The instance to use when the <see cref="ServiceFactory"/> produces an instance of <typeparamref name="TService"/>.</param>
        /// <returns>Itself.</returns>
        public InvocationServiceCollection WithService<TService>(TService implementationInstance)
        {
            this.m_ServicesByType[typeof(TService)] = implementationInstance;
            return this;
        }

        #endregion Methods

        #region - - - - - - Operators - - - - - -

        /// <summary>
        /// Converts the specified <paramref name="invocationServiceCollection"/> to a <see cref="ServiceFactory"/>.
        /// </summary>
        /// <param name="invocationServiceCollection">The <see cref="InvocationServiceCollection"/> to convert.</param>
        public static implicit operator ServiceFactory(InvocationServiceCollection invocationServiceCollection)
            => invocationServiceCollection.GetService;

        #endregion Operators

    }

}
