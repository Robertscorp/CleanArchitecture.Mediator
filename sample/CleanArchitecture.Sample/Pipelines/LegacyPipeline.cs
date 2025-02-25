using CleanArchitecture.Mediator;

namespace CleanArchitecture.Sample.Pipelines
{

    public class LegacyPipeline : Pipeline
    {

        #region - - - - - - Constructors - - - - - -

        public LegacyPipeline(ServiceFactory serviceFactory) : base(serviceFactory) { }

        #endregion Constructors

    }

}
