using System;

namespace CleanArchitecture.Mediator.Internal
{

    internal interface IPipelineHandleAccessor
    {

        #region - - - - - - Methods - - - - - -

        IPipeHandle GetPipeHandle(ServiceFactory serviceFactory);

        #endregion Methods

    }

    internal class PipelineHandleAccessor<TPipeline> : IPipelineHandleAccessor where TPipeline : Pipeline
    {

        #region - - - - - - Fields - - - - - -

        private readonly Func<ServiceFactory, IPipeHandle> m_PipeHandleFactory;

        private IPipeHandle m_PipeHandle;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public PipelineHandleAccessor(Func<ServiceFactory, IPipeHandle> pipeHandleFactory)
            => this.m_PipeHandleFactory = pipeHandleFactory ?? throw new ArgumentNullException(nameof(pipeHandleFactory));

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        IPipeHandle IPipelineHandleAccessor.GetPipeHandle(ServiceFactory serviceFactory)
        {
            if (this.m_PipeHandle == null)
                this.m_PipeHandle = this.m_PipeHandleFactory(serviceFactory);

            return this.m_PipeHandle;
        }

        #endregion Methods

    }

}
