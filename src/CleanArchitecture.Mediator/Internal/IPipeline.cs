using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Internal
{

    /// <summary>
    /// A pipeline that can be configured and invoked.
    /// </summary>
    public interface IPipeline
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Invokes the pipeline.
        /// </summary>
        /// <typeparam name="TOutputPort">The type of output port.</typeparam>
        /// <param name="inputPort">The input to the pipeline.</param>
        /// <param name="outputPort">The output mechanism for the pipeline.</param>
        /// <param name="serviceFactory">The factory used to get service instances.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        Task InvokeAsync<TOutputPort>(
            IInputPort<TOutputPort> inputPort,
            TOutputPort outputPort,
            ServiceFactory serviceFactory,
            CancellationToken cancellationToken);

        #endregion Methods

    }

}
