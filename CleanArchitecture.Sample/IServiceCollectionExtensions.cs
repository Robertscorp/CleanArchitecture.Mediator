using CleanArchitecture.Sample.Pipeline;
using CleanArchitecture.Services.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Sample
{

    public static class IServiceCollectionExtensions
    {

        #region - - - - - - Methods - - - - - -

        public static IServiceCollection AddCleanArchitectureServices(this IServiceCollection serviceCollection)
        {
            CleanArchitectureServices.Register(opts =>
                _ = opts.ConfigurePipeline(pipeline =>
                            pipeline.AddAuthentication()
                                .AddAuthorisation<AuthorisationResult>()
                                .AddBusinessRuleValidation<ValidationResult>()
                                .AddInputPortValidation<ValidationResult>()
                                .AddInteractorInvocation())
                        .ScanAssemblies(typeof(Program).Assembly)
                        .SetRegistrationAction((serviceType, implementationType) =>
                            _ = serviceCollection.AddScoped(serviceType, implementationType))
                        .Validate());

            return serviceCollection;
        }

        #endregion Methods

    }

}
