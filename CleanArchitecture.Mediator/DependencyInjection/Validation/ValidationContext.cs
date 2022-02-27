using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanArchitecture.Mediator.DependencyInjection.Validation
{

    internal class ValidationContext
    {

        #region - - - - - - Fields - - - - - -

        private readonly HashSet<Type> m_GenericRegisteredServices = new HashSet<Type>();
        private readonly HashSet<Type> m_OutputPorts = new HashSet<Type>();
        private readonly List<(Type Implementation, Type Interface)> m_RegisteredInputPorts = new List<(Type Implementation, Type Interface)>();
        private readonly List<(Type PipeOutputPort, Type PipeService)> m_SingleImplementationServices = new List<(Type PipeOutputPort, Type PipeService)>();
        private readonly HashSet<Type> m_SpecificRegisteredServices = new HashSet<Type>();
        private readonly Dictionary<Type, List<(Type PipeService, Func<Type, Type, Type, Type> UseCaseServiceResolver)>> m_UseCaseServiceResolversByOutputPort = new Dictionary<Type, List<(Type PipeService, Func<Type, Type, Type, Type> UseCaseServiceResolver)>>();

        #endregion Fields

        #region - - - - - - Methods - - - - - -

        private (Type PipeOutputPort, Type[] UseCaseServices)[] GetAllUseCaseRequiredServices()
            => this.m_RegisteredInputPorts
                .SelectMany(ucip => this.GetUseCaseRequiredServices(ucip.Implementation, ucip.Interface))
                .GroupBy(popucs => popucs.PipeOutputPort)
                .Select(gpopucs => (PipeOutputPort: gpopucs.Key,
                                    RequiredUseCaseServices: gpopucs.SelectMany(popucs => popucs.UseCaseServices).ToArray()))
                .ToArray();

        public (Type OutputPort, Type[] MissingServices)[] GetMissingSingleImplementationServices()
            => this.m_SingleImplementationServices
                .Where(popps => !this.HasRegisteredImplementation(popps.PipeService))
                .GroupBy(popps => popps.PipeOutputPort)
                .Select(gpopps => (gpopps.Key, gpopps.Select(popps => popps.PipeService).ToArray()))
                .ToArray();

        public (Type OutputPort, Type[] MissingServices)[] GetMissingUseCaseServices()
            => this.GetAllUseCaseRequiredServices()
                .Select(popucs => (popucs.PipeOutputPort,
                                    UseCaseServices: popucs
                                                        .UseCaseServices
                                                        .Where(ucs => !this.HasRegisteredImplementation(ucs))
                                                        .ToArray()))
                .Where(popucs => popucs.UseCaseServices.Any())
                .ToArray();

        public (Type PipeOutputPort, Type[] AffectedUseCaseOutputPorts)[] GetUnregisteredOutputPorts()
            => this.m_RegisteredInputPorts
                .Select(ip => GetUseCaseOutputPort(ip.Interface))
                .SelectMany(ucop => GetPipeOutputPorts(ucop).Select(pop => (PipeOutputPort: pop, UseCaseOutputPort: ucop)))
                .Where(pucops => !this.m_OutputPorts.Contains(pucops.PipeOutputPort))
                .GroupBy(pucops => pucops.PipeOutputPort)
                .Select(gpucops => (gpucops.Key, gpucops.Select(pucops => pucops.UseCaseOutputPort).ToArray()))
                .ToArray();

        private static Type[] GetPipeOutputPorts(Type useCaseOutputPort)
            => useCaseOutputPort.GetInterfaces();

        private static Type GetUseCaseOutputPort(Type pipelineInputPort)
            => pipelineInputPort.GenericTypeArguments.Single();

        private (Type PipeOutputPort, Type[] UseCaseServices)[] GetUseCaseRequiredServices(Type useCaseInputPort, Type pipelineInputPort)
        {
            var _UseCaseOutputPort = GetUseCaseOutputPort(pipelineInputPort);

            return GetPipeOutputPorts(_UseCaseOutputPort)
                    .Where(pop => this.m_UseCaseServiceResolversByOutputPort.ContainsKey(pop))
                    .Select(pop => (pop, this.m_UseCaseServiceResolversByOutputPort[pop]
                                            .Select(psucsr => psucsr.UseCaseServiceResolver(useCaseInputPort, _UseCaseOutputPort, psucsr.PipeService))
                                            .ToArray()))
                    .ToArray();
        }

        private bool HasRegisteredImplementation(Type type)
            => this.m_SpecificRegisteredServices.Contains(type) ||
                this.m_GenericRegisteredServices.Contains(type.GetTypeDefinition());

        public void RegisterAssemblyType(Type type)
        {
            foreach (var _Interface in type.GetInterfaces())
                if (Equals(typeof(IUseCaseInputPort<>), _Interface.GetTypeDefinition()))
                    this.m_RegisteredInputPorts.Add((type, _Interface));

                else
                    _ = type.IsGenericType
                            ? this.m_GenericRegisteredServices.Add(_Interface.GetTypeDefinition())
                            : this.m_SpecificRegisteredServices.Add(_Interface);
        }

        public void RegisterOutputPort(Type pipeOutputPort)
            => this.m_OutputPorts.Add(pipeOutputPort);

        public void RegisterSingleImplementationService(Type pipeOutputPort, Type pipeService)
            => this.m_SingleImplementationServices.Add((pipeOutputPort, pipeService));

        public void RegisterUseCaseService(Type pipeOutputPort, Type pipeService, Func<Type, Type, Type, Type> useCaseServiceResolver)
        {
            if (!this.m_UseCaseServiceResolversByOutputPort.TryGetValue(pipeOutputPort, out var _UseCaseServiceResolvers))
            {
                _UseCaseServiceResolvers = new List<(Type PipeService, Func<Type, Type, Type, Type> UseCaseServiceResolver)>();
                this.m_UseCaseServiceResolversByOutputPort.Add(pipeOutputPort, _UseCaseServiceResolvers);
            }

            _UseCaseServiceResolvers.Add((pipeService, useCaseServiceResolver));
        }

        #endregion Methods

    }

}
