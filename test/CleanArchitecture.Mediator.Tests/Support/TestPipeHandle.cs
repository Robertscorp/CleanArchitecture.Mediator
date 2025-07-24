using CleanArchitecture.Mediator.Internal;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Tests.Support;

internal class TestPipeHandle : IPipeHandle
{

    #region - - - - - - Fields - - - - - -

    private readonly HashSet<(object, object, ServiceFactory, CancellationToken)> m_Invocations = [];

    #endregion Fields

    #region - - - - - - Methods - - - - - -

    Task IPipeHandle.InvokePipeAsync<TInputPort, TOutputPort>(
        TInputPort inputPort,
        TOutputPort outputPort,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
    {
        _ = this.m_Invocations.Add((inputPort, outputPort, serviceFactory, cancellationToken));
        return Task.CompletedTask;
    }

    public void Verify(object inputPort, object outputPort, ServiceFactory serviceFactory, CancellationToken cancellationToken)
    {
        if (!this.m_Invocations.Remove((inputPort, outputPort, serviceFactory, cancellationToken)))
            throw new Exception("Expected invocation not found.");
    }

    public void VerifyNoOtherCalls()
    {
        if (this.m_Invocations.Count > 0)
            throw new Exception($"{this.m_Invocations.Count} unverified invocations.");
    }

    #endregion Methods

}
