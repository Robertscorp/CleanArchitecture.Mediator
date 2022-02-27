using System;

namespace CleanArchitecture.Mediator.Infrastructure
{

    /// <summary>
    /// The delegate used to get the service object of the specified type.
    /// </summary>
    /// <param name="serviceType">The type of service to resolve.</param>
    /// <returns>The service that was produced.</returns>
    public delegate object UseCaseServiceResolver(Type serviceType);

    /// <summary>
    /// Contains UseCaseServiceResolver extension methods.
    /// </summary>
    public static class UseCaseServiceResolverExtensions
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="TService">The type of the service to get.</typeparam>
        /// <param name="serviceResolver">The resolver to get the service from.</param>
        /// <returns>The service that was produced.</returns>
        public static TService GetService<TService>(this UseCaseServiceResolver serviceResolver)
            => (TService)serviceResolver(typeof(TService));

        #endregion Methods

    }

}
