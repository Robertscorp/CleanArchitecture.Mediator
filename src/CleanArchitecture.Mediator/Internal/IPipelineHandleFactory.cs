namespace CleanArchitecture.Mediator.Internal
{

    /// <summary>
    /// A factory that provides an invokable handle for a pipeline.
    /// </summary>
    public interface IPipelineHandleFactory
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Gets an invokable handle for the pipeline.
        /// </summary>
        /// <param name="pipeline">The pipeline to get the <see cref="PipeHandle"/> for.</param>
        /// <returns>The invokable handle for the pipeline.</returns>
        PipeHandle GetPipelineHandle(IPipeline pipeline);

        #endregion Methods

    }

}
