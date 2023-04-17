using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanArchitecture.Mediator.Configuration
{

    internal class PipelineConfiguration
    {

        #region - - - - - - Fields - - - - - -

        private PipeHandle m_PipeHandle;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        internal PipelineConfiguration(Type pipelineType)
            => this.PipelineType = pipelineType;

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        internal Type PipelineType { get; }

        internal List<Func<IDictionary<Type, IPipe>, IPipe>> PipeFactories { get; private set; } = new List<Func<IDictionary<Type, IPipe>, IPipe>>();

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        public PipeHandle GetPipeHandle(ServiceFactory serviceFactory)
        {
            if (this.m_PipeHandle == null)
            {
                var _PipesByType = serviceFactory.GetService<IEnumerable<IPipe>>().ToDictionary(p => p.GetType());

                this.m_PipeHandle = this.PipeFactories
                                        .Select(f => f.Invoke(_PipesByType))
                                        .Where(p => p != null)
                                        .Reverse()
                                        .Aggregate(
                                            new PipeHandle(null, null),
                                            (nextPipeHandle, pipe) => new PipeHandle(pipe, nextPipeHandle));

                this.PipeFactories = null;
            }

            return this.m_PipeHandle;
        }

        #endregion Methods

    }

}
