using CleanArchitecture.Services.DependencyInjection.Validation;

namespace CleanArchitecture.Services.DependencyInjection
{

    public class ElementOptions
    {

        #region - - - - - - Constructors - - - - - -

        public ElementOptions(Type elementType)
            => this.ElementType = elementType;

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        internal Type ElementType { get; }

        internal List<Type> Services { get; } = new();

        internal ElementValidationOptions? ValidationOptions { get; private set; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Registers a service as being required by the pipe.
        /// </summary>
        /// <typeparam name="TService">The type of service required by the pipe.</typeparam>
        /// <returns>Itself.</returns>
        public ElementOptions AddService<TService>()
            => this.AddService(typeof(TService));

        /// <summary>
        /// Registers a service as being required by the pipe.
        /// </summary>
        /// <param name="service">The type of service required by the pipe.</param>
        /// <returns>Itself.</returns>
        public ElementOptions AddService(Type service)
        {
            this.Services.Add(service);
            return this;
        }

        /// <summary>
        /// Allows this pipe to be validated for missing service implementations.
        /// </summary>
        /// <typeparam name="TOutputPort">The type of the Use Case's Output Port.</typeparam>
        /// <param name="requiredServiceTypesResolver">A function to return all services required by the pipe.</param>
        public void WithValidation<TOutputPort>(Func<Type[]> requiredServiceTypesResolver)
            => this.WithValidation(typeof(TOutputPort), requiredServiceTypesResolver);

        /// <summary>
        /// Allows this pipe to be validated for missing service implementations.
        /// </summary>
        /// <param name="outputPort">The type of the Use Case's Output Port.</param>
        /// <param name="requiredServiceTypesResolver">A function to return all services required by the pipe.</param>
        public void WithValidation(Type outputPort, Func<Type[]> requiredServiceTypesResolver)
            => this.ValidationOptions = ElementValidationOptions.FromServiceResolver(outputPort, requiredServiceTypesResolver);

        /// <summary>
        /// Allows this pipe to be validated for missing service implementations.
        /// </summary>
        /// <typeparam name="TOutputPort">The type of the Use Case's Output Port.</typeparam>
        /// <param name="requiredServiceTypesResolver">A function to return all services required by the pipe for the specified Input Port and Output Port.</param>
        public void WithValidation<TOutputPort>(Func<Type, Type, Type[]> requiredServiceTypesResolver)
            => this.WithValidation(typeof(TOutputPort), requiredServiceTypesResolver);

        /// <summary>
        /// Allows this pipe to be validated for missing service implementations.
        /// </summary>
        /// <param name="outputPort">The type of the Use Case's Output Port.</param>
        /// <param name="requiredServiceTypesResolver">A function to return all services required by the pipe for the specified Input Port and Output Port.</param>
        public void WithValidation(Type outputPort, Func<Type, Type, Type[]> requiredServiceTypesResolver)
            => this.ValidationOptions = ElementValidationOptions.FromServiceResolver(outputPort, requiredServiceTypesResolver);

        #endregion Methods

    }

}
