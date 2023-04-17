using System;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// The factory used to produce the service object of the specified type.
    /// </summary>
    /// <param name="serviceType">The type of service to produce.</param>
    /// <returns>The service that was produced.</returns>
    public delegate object ServiceFactory(Type serviceType);

    /// <summary>
    /// Contains <see cref="ServiceFactory"/> extension methods.
    /// </summary>
    public static class ServiceFactoryExtensions
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Produces the service object of the specified type.
        /// </summary>
        /// <typeparam name="TService">The type of service to produce.</typeparam>
        /// <param name="serviceFactory">The factory used to produce the service.</param>
        /// <returns>The service that was produced.</returns>
        public static TService GetService<TService>(this ServiceFactory serviceFactory)
            => (TService)serviceFactory(typeof(TService));

        #endregion Methods

    }

}
