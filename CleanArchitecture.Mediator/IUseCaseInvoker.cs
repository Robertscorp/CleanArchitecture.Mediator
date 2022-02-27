using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// A service used to invoke a Use Case.
    /// </summary>
    public interface IUseCaseInvoker
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Invokes the Use Case with the specified Input and Output Ports.
        /// </summary>
        /// <typeparam name="TUseCaseOutputPort">The type of the Use Case's Output Port.</typeparam>
        /// <param name="inputPort">The Use Case's Input Port.</param>
        /// <param name="outputPort">The Use Case's Output Port.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        Task InvokeUseCaseAsync<TUseCaseOutputPort>(
            IUseCaseInputPort<TUseCaseOutputPort> inputPort,
            TUseCaseOutputPort outputPort,
            CancellationToken cancellationToken);

        #endregion Methods

    }

}
