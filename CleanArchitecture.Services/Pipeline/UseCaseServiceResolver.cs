namespace CleanArchitecture.Services.Pipeline
{

    public delegate object? UseCaseServiceResolver(Type serviceType);

    public static class UseCaseServiceResolverExtensions
    {

        #region - - - - - - Methods - - - - - -

        public static TService? GetService<TService>(this UseCaseServiceResolver serviceResolver)
            => (TService?)serviceResolver(typeof(TService));

        #endregion Methods

    }

}
