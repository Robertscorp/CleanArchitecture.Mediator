using CleanArchitecture.Mediator;
using CleanArchitecture.Mediator.Configuration;
using CleanArchitecture.Mediator.Sample.Legacy.Authorisation;
using CleanArchitecture.Mediator.Sample.Legacy.BusinessRuleValidation;
using CleanArchitecture.Mediator.Sample.Legacy.InputPortValidation;
using CleanArchitecture.Mediator.Sample.Pipelines;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Mediator.Sample
{

    public static class IServiceCollectionExtensions
    {

        #region - - - - - - Methods - - - - - -

        public static IServiceCollection AddCleanArchitectureMediator(this IServiceCollection serviceCollection)
        {
            CleanArchitectureMediator.Configure(builder =>
            {
                _ = builder.AddPipeline<DefaultPipeline>(pipeline
                    => pipeline
                        .AddPipe(async (inputPort, outputPort, serviceFactory, nextPipeHandleAsync, cancellationToken) =>
                        {
                            Console.WriteLine("\t- Beginning invocation of DefaultPipeline.");
                            await nextPipeHandleAsync();
                            Console.WriteLine("\t- Completed invocation of DefaultPipeline.");
                        })
                        .AddAuthentication()
                        .AddAuthorisation()
                        .AddValidation()
                        .AddInteractorInvocation());

                _ = builder.AddPipeline<LegacyPipeline>(pipeline
                    => pipeline
                        .AddPipe(async (inputPort, outputPort, serviceFactory, nextPipeHandleAsync, cancellationToken) =>
                        {
                            Console.WriteLine("\t- Beginning invocation of LegacyPipeline.");
                            await nextPipeHandleAsync();
                            Console.WriteLine("\t- Completed invocation of LegacyPipeline.");
                        })
                        .AddAuthentication()
                        .AddPipe<AuthorisationPipe<AuthorisationResult>>(typeof(Legacy.Authorisation.IAuthorisationEnforcer<,>))
                        .AddPipe<InputPortValidationPipe<InputPortValidationResult>>(typeof(IInputPortValidator<,>))
                        .AddPipe<BusinessRuleValidationPipe<BusinessRuleValidationResult>>(typeof(IBusinessRuleValidator<,>))
                        .AddInteractorInvocation());

                _ = builder.AddPipeline<VerificationPipeline>(pipeline
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
            },
            registerSingletonServiceAction: (serviceType, implementationType) => serviceCollection.AddSingleton(serviceType, implementationType),
            registerSingletonFactoryAction: (serviceType, getServiceFunc) => serviceCollection.AddSingleton(serviceType, serviceProvider => getServiceFunc(serviceProvider.GetRequiredService<ServiceFactory>())),
            assembliesContainingServiceImplementations: typeof(Program).Assembly);

            _ = serviceCollection.AddScoped<ServiceFactory>(serviceProvider => serviceProvider.GetService);

            return serviceCollection;
        }

        #endregion Methods

    }

}
