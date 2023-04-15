using CleanArchitecture.Mediator;

namespace CleanArchitecture.Sample.Pipelines
{

    public class VerificationPipeline : Mediator.Pipeline
    {

        #region - - - - - - Constructors - - - - - -

        public VerificationPipeline(ServiceFactory serviceFactory) : base(serviceFactory) { }

        #endregion Constructors

    }

}
