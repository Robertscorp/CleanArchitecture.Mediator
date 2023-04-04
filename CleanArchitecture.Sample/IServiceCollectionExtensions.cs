using CleanArchitecture.Mediator.DependencyInjection;
using CleanArchitecture.Sample.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Sample
{

    public static class IServiceCollectionExtensions
    {

        #region - - - - - - Methods - - - - - -

        public static IServiceCollection AddCleanArchitectureMediator(this IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }

        #endregion Methods

    }

}
