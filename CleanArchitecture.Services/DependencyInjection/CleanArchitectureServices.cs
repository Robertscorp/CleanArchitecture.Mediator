using CleanArchitecture.Services.DependencyInjection.Validation;
using CleanArchitecture.Services.Infrastructure;
using CleanArchitecture.Services.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanArchitecture.Services.DependencyInjection
{

    /// <summary>
    /// Contains the behaviour to register Clean Architecture Mediator services.
    /// </summary>
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

            var _ServiceTypes = new HashSet<Type>(_ConfigurationOptions
                                                    .PipelineOptions
                                                    .ElementOptions
                                                    .SelectMany(e => e.PipeServiceOptions)
                                                    .Select(s => s.PipeService.GetTypeDefinition()));

            // Scan all specified Assemblies for classes that implement the Use Case Element Services.
            foreach (var _Type in _ConfigurationOptions.GetAssemblyTypes().Where(t => !t.IsAbstract))
                foreach (var _InterfaceType in _Type.GetInterfaces())
                    if (_ServiceTypes.Contains(_InterfaceType.GetTypeDefinition()))
                        if (_Type.IsGenericTypeDefinition)
                            _ConfigurationOptions.RegistrationAction?.Invoke(_InterfaceType.GetTypeDefinition(), _Type);

                        else
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

                    _Context.RegisterOutputPort(_PipeOptions.PipeOutputPort);

                    foreach (var _PipeServiceOption in _PipeServiceOptions.Where(pso => pso.UseCaseServiceResolver == null))
                        _Context.RegisterSingleImplementationService(
                            _PipeOptions.PipeOutputPort,
                            _PipeServiceOption.PipeService);

                    foreach (var _PipeServiceOption in _PipeServiceOptions.Where(pso => pso.UseCaseServiceResolver != null))
                        _Context.RegisterUseCaseService(
                            _PipeOptions.PipeOutputPort,
                            _PipeServiceOption.PipeService,
                            _PipeServiceOption.UseCaseServiceResolver);
                }

            foreach (var _IgnoredOutputPort in options.IgnoredOutputPorts)
                _Context.RegisterOutputPort(_IgnoredOutputPort);

            var _ExceptionBuilder = new ValidationExceptionBuilder();

            foreach (var (_, _MissingSingleImplementationServices) in _Context.GetMissingSingleImplementationServices())
                _ExceptionBuilder.AddMissingSingleImplementationServices(_MissingSingleImplementationServices);

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
