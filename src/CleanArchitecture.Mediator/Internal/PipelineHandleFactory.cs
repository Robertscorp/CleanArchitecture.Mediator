using CleanArchitecture.Mediator.Configuration;
using System;
using System.Linq;

namespace CleanArchitecture.Mediator.Internal
{

    /// <summary>
    /// A factory that provides an invokable handle for a pipeline.
    /// </summary>
    public class PipelineHandleFactory : IPipelineHandleFactory
    {

        #region - - - - - - Fields - - - - - -

        private readonly ServiceFactory m_ServiceFactory;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Initialises a new instance of the <see cref="PipelineHandleFactory"/> class.
        /// </summary>
        /// <param name="serviceFactory">The factory used to get the service object of the specified type.</param>
        public PipelineHandleFactory(ServiceFactory serviceFactory)
            => this.m_ServiceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        PipeHandle IPipelineHandleFactory.GetPipelineHandle(IPipeline pipeline)
            => pipeline != null
                ? this.m_ServiceFactory
                    .GetService<PackageConfiguration>()
                    .PipelineConfigurations
                    .SingleOrDefault(c => Equals(c.PipelineType, pipeline.GetType()))?
                    .GetPipeHandle(this.m_ServiceFactory)
                : throw new ArgumentNullException(nameof(pipeline));

        #endregion Methods

    }

}
