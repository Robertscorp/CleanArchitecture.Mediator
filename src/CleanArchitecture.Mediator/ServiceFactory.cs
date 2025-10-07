using System;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Represents the method to get an instance that can be referenced by the specified <paramref name="serviceType"/>.
    /// </summary>
    /// <param name="serviceType">The type of service to get an instance of.</param>
    /// <returns>An instance that can be referenced by the specified <paramref name="serviceType"/>.</returns>
    public delegate object ServiceFactory(Type serviceType);

    /// <summary>
    /// Contains <see cref="ServiceFactory"/> extension methods.
    /// </summary>
    public static class ServiceFactoryExtensions
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Gets an instance that can be referenced by the <typeparamref name="TService"/> type.
        /// </summary>
        /// <typeparam name="TService">The type of service to get an instance of.</typeparam>
        /// <param name="serviceFactory">The <see cref="ServiceFactory"/> used to get service instances. Cannot be null.</param>
        /// <returns>A <typeparamref name="TService"/> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceFactory"/> is null.</exception>
        public static TService GetService<TService>(this ServiceFactory serviceFactory)
            => (TService)(serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory)))(typeof(TService));

        #endregion Methods

    }

}
