namespace CleanArchitecture.Mediator.Internal
{

    internal interface IPipelineHandleAccessor
    {

        #region - - - - - - Properties - - - - - -

        IPipeHandle PipeHandle { get; }

        #endregion Properties

    }

    internal class PipelineHandleAccessor<TPipeline> : IPipelineHandleAccessor where TPipeline : Pipeline
    {

        #region - - - - - - Constructors - - - - - -

        public PipelineHandleAccessor(IPipeHandle pipeHandle)
            => this.PipeHandle = pipeHandle ?? throw new System.ArgumentNullException(nameof(pipeHandle));

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        public IPipeHandle PipeHandle { get; }

        #endregion Properties

    }

}
