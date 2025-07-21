using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// A single pipe in the pipeline.
    /// </summary>
    /// <remarks>
    /// Pipe implementations are registered as singletons, which means that only singleton and transient services should be resolved in their constructors.<br/>
    /// Scoped services can be resolved in the <see cref="InvokeAsync{TInputPort, TOutputPort}(TInputPort, TOutputPort, ServiceFactory, IPipeHandle, CancellationToken)"/>
    /// method by using the <see cref="ServiceFactory"/> parameter.
    /// </remarks>
    public interface IPipe
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Handles the relevant behaviour for this section of the pipeline.
        /// </summary>
        /// <typeparam name="TInputPort">The type of input port.</typeparam>
        /// <typeparam name="TOutputPort">The type of output port.</typeparam>
        /// <param name="inputPort">The input to the pipeline.</param>
        /// <param name="outputPort">The output mechanism for the pipeline.</param>
        /// <param name="serviceFactory">The factory used to get the service object of the specified type.</param>
        /// <param name="nextPipeHandle">The handle to the next pipe in the pipeline.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        Task InvokeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            IPipeHandle nextPipeHandle,
            CancellationToken cancellationToken) where TInputPort : IInputPort<TOutputPort>;

        #endregion Methods

    }

    /// <summary>
    /// A single pipe in the pipeline.
    /// </summary>
    /// <typeparam name="TInputPort">The type of input port.</typeparam>
    /// <typeparam name="TOutputPort">The type of output port.</typeparam>
    /// <remarks>
    /// Pipe implementations are registered as singletons, which means that only singleton and transient services should be resolved in their constructors.<br/>
    /// Scoped services can be resolved in the <see cref="InvokeAsync(TInputPort, TOutputPort, ServiceFactory, IPipeHandle, CancellationToken)"/>
    /// method by using the <see cref="ServiceFactory"/> parameter.
    /// </remarks>
    public interface IPipe<TInputPort, TOutputPort> where TInputPort : IInputPort<TOutputPort>
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Handles the relevant behaviour for this section of the pipeline.
        /// </summary>
        /// <param name="inputPort">The input to the pipeline.</param>
        /// <param name="outputPort">The output mechanism for the pipeline.</param>
        /// <param name="serviceFactory">The factory used to get the service object of the specified type.</param>
        /// <param name="nextPipeHandle">The handle to the next pipe in the pipeline.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        Task InvokeAsync(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            IPipeHandle nextPipeHandle,
            CancellationToken cancellationToken);

        #endregion Methods

    }

}
