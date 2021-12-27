namespace CleanArchitecture.Services.DependencyInjection
{

    public class PipeServiceOptions
    {

        #region - - - - - - Constructors - - - - - -

        internal PipeServiceOptions(Type serviceType)
            => this.PipeService = serviceType;

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        internal Type PipeService { get; }

        internal Func<Type, Type, Type, Type>? UseCaseServiceResolver { get; private set; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        internal void WithUseCaseServiceResolver(Func<Type, Type, Type, Type> useCaseServiceResolver)
            => this.UseCaseServiceResolver = useCaseServiceResolver;

        #endregion Methods

    }

}
