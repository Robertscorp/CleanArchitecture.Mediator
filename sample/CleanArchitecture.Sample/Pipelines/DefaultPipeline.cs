using CleanArchitecture.Mediator;
using CleanArchitecture.Mediator.Internal;

namespace CleanArchitecture.Sample.Pipelines
{

    public class DefaultPipeline : Pipeline
    {

        #region - - - - - - Constructors - - - - - -

        public DefaultPipeline(IPipelineHandleFactory pipelineHandleFactory) : base(pipelineHandleFactory) { }

        #endregion Constructors

    }

}
