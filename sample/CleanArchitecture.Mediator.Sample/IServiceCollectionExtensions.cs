using CleanArchitecture.Mediator;
using CleanArchitecture.Mediator.Sample.Legacy.Authorisation;
using CleanArchitecture.Mediator.Sample.Legacy.BusinessRuleValidation;
using CleanArchitecture.Mediator.Sample.Pipelines;
using CleanArchitecture.Mediator.Sample.Services;
using CleanArchitecture.Mediator.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Mediator.Sample;

public static class IServiceCollectionExtensions
{

    #region - - - - - - Methods - - - - - -

    public static IServiceCollection AddCleanArchitectureMediator(this IServiceCollection serviceCollection)
    {
        CleanArchitectureMediator.Setup(config =>
        {
            _ = config.AddPipeline<DefaultPipeline>(pipeline
                => pipeline
                    .AddPipe(async (inputPort, outputPort, serviceFactory, nextPipeHandleAsync, cancellationToken) =>
                    {
                        Console.WriteLine("\t- Beginning invocation of DefaultPipeline.");
                        await nextPipeHandleAsync();
                        Console.WriteLine("\t- Completed invocation of DefaultPipeline.");
                    })
                    .AddSingleTenantAuthentication()
                    .AddAuthorisation()
                    .AddValidation()
                    .AddInteractorInvocation());

            _ = config.AddPipeline<LegacyPipeline>(pipeline
                => pipeline
                    .AddPipe(async (inputPort, outputPort, serviceFactory, nextPipeHandleAsync, cancellationToken) =>
                    {
                        Console.WriteLine("\t- Beginning invocation of LegacyPipeline.");
                        await nextPipeHandleAsync();
                        Console.WriteLine("\t- Completed invocation of LegacyPipeline.");
                    })
                    .AddAuthentication()
                    .AddPipe<AuthorisationPipe<AuthorisationResult>>(config => config.AddSingletonService(typeof(Legacy.Authorisation.IAuthorisationEnforcer<,>)))
                    .AddInputPortValidation<InputPortValidationResult>()
                    .AddPipe<BusinessRuleValidationPipe<BusinessRuleValidationResult>>(config => config.AddSingletonService(typeof(IBusinessRuleValidator<,>)))
                    .AddInteractorInvocation());

            _ = config.AddPipeline<VerificationPipeline>(pipeline
                => pipeline
                    .AddPipe(async (inputPort, outputPort, serviceFactory, nextPipeHandleAsync, cancellationToken) =>
                    {
                        Console.WriteLine("\t- Beginning invocation of VerificationPipeline.");
                        await nextPipeHandleAsync();
                        Console.WriteLine("\t- Completed invocation of VerificationPipeline.");
                    })
                    .AddAuthentication()
                    .AddAuthorisation()
                    .AddValidation()
                    .AddPipe<VerificationSuccessPipe>());
        }, registration =>
            registration
                .AddAssemblies(typeof(Program).Assembly)
                .WithSingletonFactoryRegistrationAction((serviceType, getServiceFunc) => serviceCollection.AddSingleton(serviceType, serviceProvider => getServiceFunc(serviceProvider.GetRequiredService<ServiceFactory>())))
                .WithSingletonServiceRegistrationAction((serviceType, implementationType) => serviceCollection.AddSingleton(serviceType, implementationType)));

        _ = serviceCollection.AddScoped<ServiceFactory>(serviceProvider => serviceProvider.GetService);

        return serviceCollection;
    }

    #endregion Methods

}
