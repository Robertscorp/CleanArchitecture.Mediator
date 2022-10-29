﻿using System;

namespace CleanArchitecture.Mediator.Pipeline
{

    /// <summary>
    /// The factory used to get the service object of the specified type.
    /// </summary>
    /// <param name="serviceType">The type of service to produce.</param>
    /// <returns>The service that was produced.</returns>
    public delegate object ServiceFactory(Type serviceType);

    /// <summary>
    /// Contains ServiceFactory extension methods.
    /// </summary>
    public static class ServiceFactoryExtensions
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="TService">The type of the service to produce.</typeparam>
        /// <param name="serviceFactory">The factory used to produce the service.</param>
        /// <returns>The service that was produced.</returns>
        public static TService GetService<TService>(this ServiceFactory serviceFactory)
            => (TService)serviceFactory(typeof(TService));

        #endregion Methods

    }

}
