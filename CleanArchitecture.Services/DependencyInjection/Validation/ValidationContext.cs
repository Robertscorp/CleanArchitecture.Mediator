namespace CleanArchitecture.Services.DependencyInjection.Validation
{

    internal class ValidationContext
    {

        #region - - - - - - Fields - - - - - -

        private readonly List<(Type Implementation, Type Interface)> m_RegisteredInputPorts = new();
        private readonly HashSet<Type> m_RegisteredServices = new();
        private readonly Dictionary<Type, Func<Type, Type, Type[]>> m_UseCaseServiceResolverByOutputPort = new();

        #endregion Fields

        #region - - - - - - Methods - - - - - -

        private static Type GetGenericTypeDefinition(Type type)
            => type.IsGenericType ? type.GetGenericTypeDefinition() : type;

        private static Type GetUseCaseOutputPort(Type inputPortType)
            => inputPortType.GenericTypeArguments.Single();

        public void RegisterAssemblyType(Type type)
        {
            foreach (var _Interface in type.GetInterfaces())
                if (Equals(typeof(IUseCaseInputPort<>), GetGenericTypeDefinition(_Interface)))
                    this.m_RegisteredInputPorts.Add((type, _Interface));
                else
                    _ = this.m_RegisteredServices.Add(_Interface);
        }

        public void RegisterUseCaseServiceResolver(Type outputPortType, Func<Type, Type, Type[]> serviceResolver)
            => this.m_UseCaseServiceResolverByOutputPort
                .Add(GetGenericTypeDefinition(outputPortType), serviceResolver);

        public (Type OutputPort, Type[] MissingServices)[] GetMissingServices()
        {
            var _UseCaseRequiredServices
                = this.m_RegisteredInputPorts
                    .SelectMany(ip => GetUseCaseOutputPort(ip.Interface)
                                        .GetInterfaces()
                                        .Select(op => (InputPort: ip, OutputPort: op)))
                    .Where(ipop => this.m_UseCaseServiceResolverByOutputPort
                                    .ContainsKey(GetGenericTypeDefinition(ipop.OutputPort)))
                    .Select(ipop => (ipop.InputPort,
                                    RequiredServices: this.m_UseCaseServiceResolverByOutputPort[GetGenericTypeDefinition(ipop.OutputPort)]
                                                        .Invoke(ipop.InputPort.Implementation, ipop.OutputPort)))
                    .SelectMany(iprs => iprs.RequiredServices
                                            .Select(rs => (iprs.InputPort, RequiredService: rs)))
                    .ToArray();

            var _MissingServices
                = _UseCaseRequiredServices
                    .Where(iprs => !this.m_RegisteredServices.Contains(iprs.RequiredService))
                    .GroupBy(iprs => iprs.InputPort.Interface)
                    .Select(giprs => (OutputPort: GetUseCaseOutputPort(giprs.Key),
                                        MissingServices: giprs.Select(iprs => iprs.RequiredService).ToArray()))
                    .ToArray();

            return _MissingServices;
        }

        #endregion Methods

    }

}
