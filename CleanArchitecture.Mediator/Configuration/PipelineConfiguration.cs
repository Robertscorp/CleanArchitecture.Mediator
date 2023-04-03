using CleanArchitecture.Mediator.Pipeline;
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

        internal List<Type> PipeTypes { get; } = new List<Type>();

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        public PipeHandle GetPipeHandle(ServiceFactory serviceFactory)
        {
            if (this.m_PipeHandle == null)
            {
                var _PipesByType = serviceFactory.GetService<IEnumerable<IPipe>>().ToDictionary(p => p.GetType());

                this.m_PipeHandle = this.PipeTypes
                                        .Select(t => _PipesByType.TryGetValue(t, out var _Pipe) ? _Pipe : null)
                                        .Where(p => p != null)
                                        .Reverse()
                                        .Aggregate(
                                            new PipeHandle(null, null),
                                            (nextPipeHandle, useCasePipe) => new PipeHandle(useCasePipe, nextPipeHandle));
            }

            return this.m_PipeHandle;
        }

        #endregion Methods

    }

}
