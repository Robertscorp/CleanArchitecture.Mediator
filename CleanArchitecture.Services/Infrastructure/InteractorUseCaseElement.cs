﻿using CleanArchitecture.Services.Pipeline;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Services.Infrastructure
{

    /// <summary>
    /// Handles invocation of the Interactor service.
    /// </summary>
    public class InteractorUseCaseElement : IUseCaseElement
    {

        #region - - - - - - Fields - - - - - -

        private readonly UseCaseServiceResolver m_ServiceResolver;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Initialises a new instance of the <see cref="InteractorUseCaseElement"/> class.
        /// </summary>
        /// <param name="serviceResolver">The delegate used to get services.</param>
        public InteractorUseCaseElement(UseCaseServiceResolver serviceResolver)
            => this.m_ServiceResolver = serviceResolver ?? throw new ArgumentNullException(nameof(serviceResolver));

        #endregion Constructors

        #region - - - - - - IUseCaseElement Implementation - - - - - -

        Task IUseCaseElement.HandleAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            UseCaseElementHandleAsync nextUseCaseElementHandle,
            CancellationToken cancellationToken)
            => this.m_ServiceResolver.GetService<IUseCaseInteractor<TUseCaseInputPort, TUseCaseOutputPort>>()?
                .HandleAsync(inputPort, outputPort, cancellationToken)
                    ?? Task.CompletedTask;

        #endregion IUseCaseElement Implementation

    }

}
