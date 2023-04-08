namespace CleanArchitecture.Mediator.Internal
{

    /// <summary>
    /// A factory that provides an invokable handle for a Pipeline.
    /// </summary>
    public interface IPipelineHandleFactory
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Gets an invokable handle for the Pipeline.
        /// </summary>
        /// <typeparam name="TPipeline">The type of Pipeline.</typeparam>
        /// <returns>The invokable handle for the Pipeline.</returns>
        PipeHandle GetPipelineHandle<TPipeline>() where TPipeline : IPipeline;

        #endregion Methods

    }

}
