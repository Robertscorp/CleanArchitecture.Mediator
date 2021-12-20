namespace CleanArchitecture.Services.DependencyInjection
{

    /// <summary>
    /// The options used to configure a pipe in the Use Case Pipeline.
    /// </summary>
    public class UseCaseElementOptions
    {

        #region - - - - - - Constructors - - - - - -

        internal UseCaseElementOptions(Type elementType)
            => this.ElementType = elementType;

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        internal Type ElementType { get; }

        internal List<Type> Services { get; } = new();

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Registers a service as being required by the pipe.
        /// </summary>
        /// <typeparam name="TService">The type of service required by the pipe.</typeparam>
        /// <returns>Itself.</returns>
        public UseCaseElementOptions AddUseCaseService<TService>()
            => this.AddUseCaseService(typeof(TService));

        /// <summary>
        /// Registers a service as being required by the pipe.
        /// </summary>
        /// <param name="service">The type of service required by the pipe.</param>
        /// <returns>Itself.</returns>
        public UseCaseElementOptions AddUseCaseService(Type service)
        {
            this.Services.Add(service);
            return this;
        }

        #endregion Methods

    }

}
