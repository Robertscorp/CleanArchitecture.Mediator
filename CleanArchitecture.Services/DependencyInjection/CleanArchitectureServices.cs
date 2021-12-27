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
                                    .SelectMany(e => e.PipeServiceOptions)
                                    .Select(s => s.PipeService.GetTypeDefinition())
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

            foreach (var _Type in options.GetAssemblyTypes())
                _Context.RegisterAssemblyType(_Type);

            foreach (var _PipeOptions in options.PipelineOptions.ElementOptions)
                if (_PipeOptions.PipeOutputPort != null)
                {
                    var _PipeServiceOptions = _PipeOptions.PipeServiceOptions;

                    _Context.RegisterPipeOutputPort(_PipeOptions.PipeOutputPort);

                    foreach (var _PipeServiceOption in _PipeServiceOptions.Where(pso => pso.UseCaseServiceResolver == null))
                        _Context.RegisterSingleImplementationService(
                            _PipeOptions.PipeOutputPort,
                            _PipeServiceOption.PipeService);

                    foreach (var _PipeServiceOption in _PipeServiceOptions.Where(pso => pso.UseCaseServiceResolver != null))
                        _Context.RegisterUseCaseService(
                            _PipeOptions.PipeOutputPort,
                            _PipeServiceOption.PipeService,
                            _PipeServiceOption.UseCaseServiceResolver!);
                }

            var _ExceptionBuilder = new ValidationExceptionBuilder();

            foreach (var (_, _MissingSingleImplementationServices) in _Context.GetMissingSingleImplementationServices())
                foreach (var _MissingSingleImplementationService in _MissingSingleImplementationServices)
                    _ExceptionBuilder.AddMissingSingleImplementationService(_MissingSingleImplementationService);

            foreach (var (_InputPort, _MissingUseCaseServices) in _Context.GetMissingUseCaseServices())
                _ExceptionBuilder.AddMissingUseCaseServices(_InputPort, _MissingUseCaseServices);

            foreach (var (_PipeOutputPort, _AffectedUseCaseOutputPorts) in _Context.GetUnregisteredOutputPorts())
                _ExceptionBuilder.AddUnregisteredOutputPort(_PipeOutputPort, _AffectedUseCaseOutputPorts);

            var _Exception = _ExceptionBuilder.ToValidationException();
            if (_Exception != null)
                throw _Exception;
        }

        #endregion Methods

    }

}
