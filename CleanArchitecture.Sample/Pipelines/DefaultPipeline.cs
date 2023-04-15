using CleanArchitecture.Mediator;

namespace CleanArchitecture.Sample.Pipelines
{

    public class DefaultPipeline : Mediator.Pipeline
    {

        #region - - - - - - Constructors - - - - - -

        public DefaultPipeline(ServiceFactory serviceFactory) : base(serviceFactory) { }

        #endregion Constructors

    }

}
