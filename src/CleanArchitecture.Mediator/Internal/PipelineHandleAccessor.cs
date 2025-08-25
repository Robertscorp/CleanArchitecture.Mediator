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

        private IPipeHandle m_PipeHandle;
        private Func<ServiceFactory, IPipeHandle> m_PipeHandleFactory;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public PipelineHandleAccessor(Func<ServiceFactory, IPipeHandle> pipeHandleFactory)
            => this.m_PipeHandleFactory = pipeHandleFactory ?? throw new ArgumentNullException(nameof(pipeHandleFactory));

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        IPipeHandle IPipelineHandleAccessor.GetPipeHandle(ServiceFactory serviceFactory)
        {
            if (this.m_PipeHandle == null)
            {
                var _Factory = this.m_PipeHandleFactory;
                if (_Factory != null) // The factory is unassigned after the handle is set. If the factory is null, the handle has been set.
                {
                    this.m_PipeHandle = _Factory(serviceFactory);
                    this.m_PipeHandleFactory = null; // Unassign the factory so it can be collected.
                }
            }

            return this.m_PipeHandle;
        }

        #endregion Methods

    }

}
