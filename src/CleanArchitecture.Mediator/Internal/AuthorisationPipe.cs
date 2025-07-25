﻿using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Internal
{

    internal class AuthorisationPipe : IPipe
    {

        #region - - - - - - Methods - - - - - -

        async Task IPipe.InvokeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            NextPipeHandleAsync nextPipeHandle,
            CancellationToken cancellationToken)
        {
            var _AuthEnforcer = serviceFactory.GetService<IAuthorisationEnforcer<TInputPort, TOutputPort>>();
            if (_AuthEnforcer == null || await _AuthEnforcer.HandleAuthorisationAsync(inputPort, outputPort, serviceFactory, cancellationToken).ConfigureAwait(false))
                await nextPipeHandle().ConfigureAwait(false);
        }

        #endregion Methods

    }

}
