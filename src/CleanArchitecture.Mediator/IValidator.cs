using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// A service used to determine if it's valid for the pipeline to continue.
    /// </summary>
    /// <typeparam name="TInputPort">The type of input port.</typeparam>
    /// <typeparam name="TOutputPort">The type of output port.</typeparam>
    public interface IValidator<TInputPort, TOutputPort> where TInputPort : IInputPort<TOutputPort>
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Determines if it's valid for the pipeline to continue.
        /// </summary>
        /// <param name="inputPort">The input to the pipeline.</param>
        /// <param name="outputPort">The output mechanism for the pipeline.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>A result indicating if it's valid for the pipeline to continue.</returns>
        Task<bool> HandleValidationAsync(TInputPort inputPort, TOutputPort outputPort, CancellationToken cancellationToken);

        #endregion Methods

    }

}
