using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Internal
{

    /// <summary>
    /// Represents an invokable handle to an <see cref="IPipe"/>.
    /// </summary>
    internal interface IPipeHandle
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Invokes the pipe with the specified parameters.
        /// </summary>
        /// <typeparam name="TInputPort">The type of input port.</typeparam>
        /// <typeparam name="TOutputPort">The type of output port.</typeparam>
        /// <param name="inputPort">The input to the pipeline.</param>
        /// <param name="outputPort">The output mechanism for the pipeline.</param>
        /// <param name="serviceFactory">The <see cref="ServiceFactory"/> used to get service instances.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        Task InvokePipeAsync<TInputPort, TOutputPort>(
            TInputPort inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            CancellationToken cancellationToken) where TInputPort : IInputPort<TOutputPort>;

        #endregion Methods

    }

}
