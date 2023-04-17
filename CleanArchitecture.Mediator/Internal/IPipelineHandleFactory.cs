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
        /// <typeparam name="TPipeline">The type of pipeline.</typeparam>
        /// <returns>The invokable handle for the pipeline.</returns>
        PipeHandle GetPipelineHandle<TPipeline>() where TPipeline : IPipeline;

        #endregion Methods

    }

}
