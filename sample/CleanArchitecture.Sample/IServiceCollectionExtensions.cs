using CleanArchitecture.Mediator;
using CleanArchitecture.Mediator.Configuration;
using CleanArchitecture.Sample.Infrastructure;
using CleanArchitecture.Sample.Pipelines;
using CleanArchitecture.Sample.UseCases.CreateProduct;
using CleanArchitecture.Sample.UseCases.GetProduct;
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
                            Console.Write("Invoking Default Pipeline...");
                            await nextPipeHandleAsync();
                            Console.WriteLine(" ...Done!");
                        })
                        .AddAuthentication()
                        .AddAuthorisation()
                        .AddValidation()
                        .AddInteractorInvocation());

                _ = builder.AddPipeline<VerificationPipeline>(pipeline
                    => pipeline
                        .AddPipe(async (inputPort, outputPort, serviceFactory, nextPipeHandleAsync, cancellationToken) =>
                        {
                            Console.Write("Invoking Verification Pipeline...");
                            await nextPipeHandleAsync();
                            Console.WriteLine(" ...Done!");
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
            _ = serviceCollection.AddScoped<IAuthorisationEnforcer<CreateProductInputPort, ICreateProductOutputPort>, CreateProductAuthorisationEnforcer>();
            _ = serviceCollection.AddScoped<IInteractor<CreateProductInputPort, ICreateProductOutputPort>, CreateProductInteractor>();
            _ = serviceCollection.AddScoped<IInteractor<GetProductInputPort, IGetProductOutputPort>, GetProductInteractor>();
            _ = serviceCollection.AddScoped<IValidator<CreateProductInputPort, ICreateProductOutputPort>, CreateProductValidator>();

            return serviceCollection;
        }

        #endregion Methods

    }

}
