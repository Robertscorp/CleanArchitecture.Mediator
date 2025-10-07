using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Provides the functionality to handle the success case for a pipeline.
    /// </summary>
    /// <typeparam name="TInputPort">The type of input port.</typeparam>
    /// <typeparam name="TOutputPort">The type of output port.</typeparam>
    public interface IInteractor<TInputPort, TOutputPort> where TInputPort : IInputPort<TOutputPort>
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Handles the success case for a pipeline.
        /// </summary>
        /// <param name="inputPort">The input to the pipeline.</param>
        /// <param name="outputPort">The output mechanism for the pipeline.</param>
        /// <param name="serviceFactory">The <see cref="ServiceFactory"/> used to get service instances.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        Task HandleAsync(TInputPort inputPort, TOutputPort outputPort, ServiceFactory serviceFactory, CancellationToken cancellationToken);

        #endregion Methods

    }

}
