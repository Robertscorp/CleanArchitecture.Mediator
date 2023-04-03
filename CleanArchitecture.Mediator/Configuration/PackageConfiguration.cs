using System.Collections.Generic;

namespace CleanArchitecture.Mediator.Configuration
{

    internal class PackageConfiguration
    {

        #region - - - - - - Constructors - - - - - -

        internal PackageConfiguration() { }

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        internal List<PipelineConfiguration> PipelineConfigurations { get; } = new List<PipelineConfiguration>();

        #endregion Properties

    }

}
