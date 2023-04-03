using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator.Pipeline
{

    /// <summary>
    /// A Pipeline that can be configured and invoked.
    /// </summary>
    public interface IPipeline
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Invokes the Pipeline.
        /// </summary>
        /// <typeparam name="TOutputPort">The type of Output Port.</typeparam>
        /// <param name="inputPort">The input to the pipeline.</param>
        /// <param name="outputPort">The output mechanism for the pipeline.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        Task InvokeAsync<TOutputPort>(
            IUseCaseInputPort<TOutputPort> inputPort,
            TOutputPort outputPort,
            CancellationToken cancellationToken);

        #endregion Methods

    }

}
