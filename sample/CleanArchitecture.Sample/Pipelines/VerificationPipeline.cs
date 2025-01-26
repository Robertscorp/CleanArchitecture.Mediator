using CleanArchitecture.Mediator;
using CleanArchitecture.Mediator.Internal;

namespace CleanArchitecture.Sample.Pipelines
{

    public class VerificationPipeline : Pipeline
    {

        #region - - - - - - Constructors - - - - - -

        public VerificationPipeline(IPipelineHandleFactory pipelineHandleFactory) : base(pipelineHandleFactory) { }

        #endregion Constructors

    }

}
