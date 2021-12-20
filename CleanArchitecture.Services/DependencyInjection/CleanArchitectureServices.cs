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

            var _ServiceTypes = _ConfigurationOptions.PipelineOptions.ElementOptions.SelectMany(e => e.Services);
            var _ServiceTypeLookup = new HashSet<Type>(_ServiceTypes.Select(s => s.IsGenericType ? s.GetGenericTypeDefinition() : s));

            foreach (var _Type in _ConfigurationOptions.AssembliesToScan.SelectMany(a => a.GetTypes()))
                foreach (var _InterfaceType in _Type.GetInterfaces())
                    if (_ServiceTypeLookup.Contains(_InterfaceType.IsGenericType
                            ? _InterfaceType.GetGenericTypeDefinition()
                            : _InterfaceType))
                        _ConfigurationOptions.RegistrationAction?.Invoke(_InterfaceType, _Type);

            foreach (var _Type in _ConfigurationOptions.PipelineOptions.ElementOptions.Select(e => e.ElementType))
                _ConfigurationOptions.RegistrationAction?.Invoke(typeof(IUseCaseElement), _Type);
        }

        #endregion Methods

    }

}
