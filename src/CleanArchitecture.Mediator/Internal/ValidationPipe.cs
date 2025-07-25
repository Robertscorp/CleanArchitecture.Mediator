﻿using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Internal
{

    internal class ValidationPipe : IPipe
    {

        #region - - - - - - Methods - - - - - -

        async Task IPipe.InvokeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            NextPipeHandleAsync nextPipeHandle,
            CancellationToken cancellationToken)
        {
            var _Validator = serviceFactory.GetService<IValidator<TInputPort, TOutputPort>>();
            if (_Validator == null || await _Validator.HandleValidationAsync(inputPort, outputPort, serviceFactory, cancellationToken).ConfigureAwait(false))
                await nextPipeHandle().ConfigureAwait(false);
        }

        #endregion Methods

    }

}
