using CleanArchitecture.Mediator;
using CleanArchitecture.Sample.OutputPorts;

namespace CleanArchitecture.Sample.Pipelines
{

    public class VerificationPipelineInvoker
    {

        #region - - - - - - Fields - - - - - -

        private readonly VerificationPipeline m_Pipeline;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public VerificationPipelineInvoker(VerificationPipeline pipeline)
            => this.m_Pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        public Task InvokeAsync<TOutputPort, TPresenter>(
            IInputPort<TOutputPort> inputPort,
            TPresenter presenter,
            ServiceFactory serviceFactory,
            CancellationToken cancellationToken) where TPresenter : IVerificationSuccessOutputPort, TOutputPort
            => this.m_Pipeline.InvokeAsync(inputPort, presenter, serviceFactory, cancellationToken);

        #endregion Methods

    }

}
