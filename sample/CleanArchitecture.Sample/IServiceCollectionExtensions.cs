using CleanArchitecture.Mediator;
using CleanArchitecture.Mediator.Configuration;
using CleanArchitecture.Sample.Infrastructure;
using CleanArchitecture.Sample.Legacy.Authorisation;
using CleanArchitecture.Sample.Legacy.BusinessRuleValidation;
using CleanArchitecture.Sample.Legacy.InputPortValidation;
using CleanArchitecture.Sample.Pipelines;
using CleanArchitecture.Sample.UseCases.CreateProduct;
using CleanArchitecture.Sample.UseCases.GetProduct;
using CleanArchitecture.Sample.UseCases.LegacyCreateProduct;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Sample
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
                        .AddPipe<AuthorisationPipe<AuthorisationResult>>()
                        .AddPipe<InputPortValidationPipe<InputPortValidationResult>>()
                        .AddPipe<BusinessRuleValidationPipe<BusinessRuleValidationResult>>()
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
            registerSingletonServiceAction: serviceType => serviceCollection.AddSingleton(serviceType),
            registerSingletonFactoryAction: (serviceType, getServiceFunc) => serviceCollection.AddSingleton(serviceType, serviceProvider => getServiceFunc(serviceProvider.GetRequiredService<ServiceFactory>())));

            _ = serviceCollection.AddScoped<ServiceFactory>(serviceProvider => serviceProvider.GetService);

            // The following services will be handled via ServiceRegistrations (returned in place of _PackageConfiguration)
            _ = serviceCollection.AddScoped<IAuthenticatedClaimsPrincipalProvider, AuthenticatedClaimsPrincipalProvider>();
            _ = serviceCollection.AddScoped<Mediator.IAuthorisationEnforcer<CreateProductInputPort, ICreateProductOutputPort>, CreateProductAuthorisationEnforcer>();
            _ = serviceCollection.AddScoped<IInteractor<CreateProductInputPort, ICreateProductOutputPort>, CreateProductInteractor>();
            _ = serviceCollection.AddScoped<IInteractor<GetProductInputPort, IGetProductOutputPort>, GetProductInteractor>();
            _ = serviceCollection.AddScoped<IValidator<CreateProductInputPort, ICreateProductOutputPort>, CreateProductValidator>();

            _ = serviceCollection.AddScoped<Legacy.Authorisation.IAuthorisationEnforcer<LegacyCreateProductInputPort, AuthorisationResult>, LegacyCreateProductAuthorisationEnforcer>();
            _ = serviceCollection.AddScoped<IInteractor<LegacyCreateProductInputPort, ILegacyCreateProductOutputPort>, LegacyCreateProductInteractor>();
            _ = serviceCollection.AddScoped<IBusinessRuleValidator<LegacyCreateProductInputPort, BusinessRuleValidationResult>, LegacyCreateProductBusinessRuleValidator>();
            _ = serviceCollection.AddScoped<IInputPortValidator<LegacyCreateProductInputPort, InputPortValidationResult>, LegacyCreateProductInputPortValidator>();

            return serviceCollection;
        }

        #endregion Methods

    }

}
