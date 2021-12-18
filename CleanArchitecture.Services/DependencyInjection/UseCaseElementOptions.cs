namespace CleanArchitecture.Services.DependencyInjection
{

    public class UseCaseElementOptions
    {

        #region - - - - - - Constructors - - - - - -

        public UseCaseElementOptions(Type elementType)
            => this.ElementType = elementType;

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        internal Type ElementType { get; }

        internal List<Type> Services { get; } = new();

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        public UseCaseElementOptions AddUseCaseService<TService>()
            => this.AddUseCaseService(typeof(TService));

        public UseCaseElementOptions AddUseCaseService(Type service)
        {
            this.Services.Add(service);
            return this;
        }

        #endregion Methods

    }

}
