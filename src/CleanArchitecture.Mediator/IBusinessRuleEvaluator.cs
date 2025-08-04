using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Provides the functionality to evaluate the business rules of the operation.
    /// </summary>
    /// <typeparam name="TInputPort">The type of input port.</typeparam>
    /// <typeparam name="TOutputPort">The type of output port.</typeparam>
    public interface IBusinessRuleEvaluator<TInputPort, TOutputPort> where TInputPort : IInputPort<TOutputPort>
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Evaluates the business rules of the operation.
        /// </summary>
        /// <param name="inputPort">The input to the pipeline.</param>
        /// <param name="outputPort">The output mechanism for the pipeline.</param>
        /// <param name="serviceFactory">The factory used to get service instances.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>A continuation strategy that determines how the pipeline should proceed.</returns>
        Task<ContinuationBehaviour> EvaluateAsync(TInputPort inputPort, TOutputPort outputPort, ServiceFactory serviceFactory, CancellationToken cancellationToken);

        #endregion Methods

    }

}
