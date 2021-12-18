using System.Reflection;

namespace CleanArchitecture.Services.DependencyInjection
{

    public class ConfigurationOptions
    {

        #region - - - - - - Properties - - - - - -

        internal List<Assembly> AssembliesToScan { get; } = new();

        internal PipelineOptions PipelineOptions { get; } = new();

        internal Action<Type, Type>? RegistrationAction { get; private set; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        public ConfigurationOptions ConfigurePipeline(Action<PipelineOptions> pipelineConfigurationAction)
        {
            pipelineConfigurationAction(this.PipelineOptions);
            return this;
        }

        public ConfigurationOptions ScanAssemblies(Assembly assemblyToScan, params Assembly[] additionalAssembliesToScan)
        {
            this.AssembliesToScan.Add(assemblyToScan);
            this.AssembliesToScan.AddRange(additionalAssembliesToScan);
            return this;
        }

        public ConfigurationOptions SetRegistrationAction(Action<Type, Type> registrationAction)
        {
            this.RegistrationAction = registrationAction;
            return this;
        }

        #endregion Methods

    }

}
