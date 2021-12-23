namespace CleanArchitecture.Services.DependencyInjection.Validation
{

    internal abstract class ElementValidationOptions
    {

        #region - - - - - - Constructors - - - - - -

        protected ElementValidationOptions(Type outputPort)
            => this.OutputPort = outputPort;

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        public Type OutputPort { get; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        internal static ElementValidationOptions FromServiceResolver(Type outputPort, Func<Type[]> requiredServiceTypesResolver)
            => new SingleImplementationValidation(outputPort, requiredServiceTypesResolver);

        internal static ElementValidationOptions FromServiceResolver(Type outputPort, Func<Type, Type, Type[]> requiredServiceTypesResolver)
            => new PerUseCaseValidation(outputPort, requiredServiceTypesResolver);

        public abstract Type[] GetRequiredServiceTypes(Type inputPort, Type outputPort);

        #endregion Methods

        #region - - - - - - Nested Classes - - - - - -

        private class PerUseCaseValidation : ElementValidationOptions
        {

            #region - - - - - - Fields - - - - - -

            private readonly Func<Type, Type, Type[]> m_RequiredServiceTypesResolver;

            #endregion Fields

            #region - - - - - - Constructors - - - - - -

            public PerUseCaseValidation(Type outputPort, Func<Type, Type, Type[]> requiredServiceTypesResolver) : base(outputPort)
                => this.m_RequiredServiceTypesResolver = requiredServiceTypesResolver;

            #endregion Constructors

            #region - - - - - - Methods - - - - - -

            public override Type[] GetRequiredServiceTypes(Type inputPort, Type outputPort)
                => this.m_RequiredServiceTypesResolver(inputPort, outputPort);

            #endregion Methods

        }

        private class SingleImplementationValidation : ElementValidationOptions
        {

            #region - - - - - - Fields - - - - - -

            private readonly Func<Type[]> m_RequiredServiceTypesResolver;

            #endregion Fields

            #region - - - - - - Constructors - - - - - -

            public SingleImplementationValidation(Type outputPort, Func<Type[]> requiredServiceTypesResolver) : base(outputPort)
                => this.m_RequiredServiceTypesResolver = requiredServiceTypesResolver;

            #endregion Constructors

            #region - - - - - - Methods - - - - - -

            public override Type[] GetRequiredServiceTypes(Type inputPort, Type outputPort)
                => this.m_RequiredServiceTypesResolver();

            #endregion Methods

        }

        #endregion Nested Classes

    }

}
