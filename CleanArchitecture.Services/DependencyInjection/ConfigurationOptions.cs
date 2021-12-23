using System.Reflection;

namespace CleanArchitecture.Services.DependencyInjection
{

    /// <summary>
    /// The options used to configure the Clean Architecture Service package.
    /// </summary>
    public class ConfigurationOptions
    {

        #region - - - - - - Constructors - - - - - -

        internal ConfigurationOptions() { }

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        private List<Assembly> AssembliesToScan { get; } = new();

        internal PipelineOptions PipelineOptions { get; } = new();

        internal Action<Type, Type>? RegistrationAction { get; private set; }

        private Type[]? ScannedTypes { get; set; }

        internal bool ShouldValidate { get; private set; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        internal Type[] GetAssemblyTypes()
            => this.ScannedTypes ??= this.AssembliesToScan.SelectMany(a => a.GetTypes()).ToArray();

        /// <summary>
        /// Configures the Use Case Pipeline.
        /// </summary>
        /// <param name="pipelineConfigurationAction">The action to configure the Use Case Pipeline.</param>
        /// <returns>Itself.</returns>
        public ConfigurationOptions ConfigurePipeline(Action<PipelineOptions> pipelineConfigurationAction)
        {
            pipelineConfigurationAction(this.PipelineOptions);
            return this;
        }

        /// <summary>
        /// Registers a collection of assemblies to scan for service implementations.
        /// </summary>
        /// <param name="assemblyToScan">An assembly to scan for service implementations.</param>
        /// <param name="additionalAssembliesToScan">Any additional assemblies to scan for service implementations.</param>
        /// <returns>Itself.</returns>
        public ConfigurationOptions ScanAssemblies(Assembly assemblyToScan, params Assembly[] additionalAssembliesToScan)
        {
            this.AssembliesToScan.Add(assemblyToScan);
            this.AssembliesToScan.AddRange(additionalAssembliesToScan);
            return this;
        }

        /// <summary>
        /// Sets the action used to register any service implementations that are found.
        /// </summary>
        /// <param name="registrationAction">The action to register a service and implementation with the service container.</param>
        /// <returns>Itself.</returns>
        public ConfigurationOptions SetRegistrationAction(Action<Type, Type> registrationAction)
        {
            this.RegistrationAction = registrationAction;
            return this;
        }

        /// <summary>
        /// Validates after registration, which will throw a <see cref="Validation.ValidationException"/> if any issues were found.
        /// </summary>
        /// <returns>Itself.</returns>
        public ConfigurationOptions Validate()
        {
            this.ShouldValidate = true;
            return this;
        }

        #endregion Methods

    }

}
