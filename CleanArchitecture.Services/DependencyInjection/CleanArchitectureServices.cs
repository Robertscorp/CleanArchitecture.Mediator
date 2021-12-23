using CleanArchitecture.Services.DependencyInjection.Validation;
using CleanArchitecture.Services.Infrastructure;
using CleanArchitecture.Services.Pipeline;

namespace CleanArchitecture.Services.DependencyInjection
{

    public static class CleanArchitectureServices
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Registers the Use Case Pipeline and supporting services.
        /// </summary>
        /// <param name="configurationAction">The action to configure the registration process.</param>
        public static void Register(Action<ConfigurationOptions> configurationAction)
        {
            var _ConfigurationOptions = new ConfigurationOptions();
            configurationAction(_ConfigurationOptions);

            _ConfigurationOptions.RegistrationAction?.Invoke(typeof(IUseCaseInvoker), typeof(UseCaseInvoker));

            var _ServiceTypes = _ConfigurationOptions
                                    .PipelineOptions
                                    .ElementOptions
                                    .SelectMany(e => e.Services)
                                    .Select(s => s.GetTypeDefinition())
                                    .ToHashSet();

            // Scan all specified Assemblies for classes that implement the Use Case Element Services.
            foreach (var _Type in _ConfigurationOptions.GetAssemblyTypes())
                foreach (var _InterfaceType in _Type.GetInterfaces())
                    if (_ServiceTypes.Contains(_InterfaceType.GetTypeDefinition()))
                        _ConfigurationOptions.RegistrationAction?.Invoke(_InterfaceType, _Type);

            foreach (var _Type in _ConfigurationOptions.PipelineOptions.ElementOptions.Select(e => e.ElementType))
                _ConfigurationOptions.RegistrationAction?.Invoke(typeof(IUseCaseElement), _Type);

            if (_ConfigurationOptions.ShouldValidate)
                Validate(_ConfigurationOptions);
        }

        private static void Validate(ConfigurationOptions options)
        {
            var _Context = new ValidationContext();

            foreach (var _ValidationOptions in options.PipelineOptions.ElementOptions.Select(e => e.ValidationOptions).Where(v => v != null))
                _Context.RegisterUseCaseServiceResolver(_ValidationOptions!.OutputPort, _ValidationOptions.GetRequiredServiceTypes);

            foreach (var _Type in options.GetAssemblyTypes())
                _Context.RegisterAssemblyType(_Type);

            var _ExceptionBuilder = new ValidationExceptionBuilder();

            foreach (var (_InputPort, _MissingServices) in _Context.GetMissingServices())
                _ExceptionBuilder.AddMissingServices(_InputPort, _MissingServices);

            var _Exception = _ExceptionBuilder.ToValidationException();
            if (_Exception != null)
                throw _Exception;
        }

        #endregion Methods

    }

}
