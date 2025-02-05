namespace CleanArchitecture.Mediator.Internal
{

    internal interface IPipelineHandleAccessor
    {

        #region - - - - - - Properties - - - - - -

        PipeHandle PipeHandle { get; }

        #endregion Properties

    }

    internal class PipelineHandleAccessor<TPipeline> : IPipelineHandleAccessor where TPipeline : Pipeline
    {

        #region - - - - - - Constructors - - - - - -

        public PipelineHandleAccessor(PipeHandle pipeHandle)
            => this.PipeHandle = pipeHandle ?? throw new System.ArgumentNullException(nameof(pipeHandle));

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        public PipeHandle PipeHandle { get; }

        #endregion Properties

    }

}
