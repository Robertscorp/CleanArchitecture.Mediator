using CleanArchitecture.Mediator;
using CleanArchitecture.Mediator.Configuration;
using CleanArchitecture.Mediator.Internal;
using CleanArchitecture.Mediator.Pipes;
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
            var _PackageConfiguration = CleanArchitectureMediator.Configure(builder =>
            {
                _ = builder.AddPipeline<DefaultPipeline>(pipeline
                    => pipeline
                        .AddPipe(async opts =>
                        {
                            Console.Write("Invoking Default Pipeline...");
                            await opts.NextPipeHandle.InvokePipeAsync();
                            Console.WriteLine(" ...Done!");
                        })
                        .AddAuthentication()
                        .AddAuthorisation<AuthorisationResult>()
                        .AddValidation<ValidationResult>()
                        .AddInteractorInvocation());

                _ = builder.AddPipeline<VerificationPipeline>(pipeline
                    => pipeline
                        .AddPipe(async opts =>
                        {
                            Console.Write("Invoking Verification Pipeline...");
                            await opts.NextPipeHandle.InvokePipeAsync();
                            Console.WriteLine(" ...Done!");
                        })
                        .AddAuthentication()
                        .AddAuthorisation<AuthorisationResult>()
                        .AddValidation<ValidationResult>()
                        .AddPipe<VerificationSuccessPipe>());
            });

            _ = serviceCollection.AddSingleton(_PackageConfiguration.GetType(), _PackageConfiguration);
            _ = serviceCollection.AddScoped<ServiceFactory>(serviceProvider => serviceProvider.GetService);

            // The following services will be handled via ServiceRegistrations (returned in place of _PackageConfiguration)
            _ = serviceCollection.AddScoped<DefaultPipeline>();
            _ = serviceCollection.AddScoped<IAuthenticatedClaimsPrincipalProvider, AuthenticatedClaimsPrincipalProvider>();
            _ = serviceCollection.AddScoped<IAuthorisationEnforcer<CreateProductInputPort, AuthorisationResult>, CreateProductAuthorisationEnforcer>();
            _ = serviceCollection.AddScoped<IInteractor<CreateProductInputPort, ICreateProductOutputPort>, CreateProductInteractor>();
            _ = serviceCollection.AddScoped<IInteractor<GetProductInputPort, IGetProductOutputPort>, GetProductInteractor>();
            _ = serviceCollection.AddScoped<IPipe, AuthenticationPipe>();
            _ = serviceCollection.AddScoped<IPipe, AuthorisationPipe<AuthorisationResult>>();
            _ = serviceCollection.AddScoped<IPipe, InteractorInvocationPipe>();
            _ = serviceCollection.AddScoped<IPipe, ValidationPipe<ValidationResult>>();
            _ = serviceCollection.AddScoped<IPipe, VerificationSuccessPipe>();
            _ = serviceCollection.AddScoped<IValidator<CreateProductInputPort, ValidationResult>, CreateProductInputPortValidator>();
            _ = serviceCollection.AddScoped<VerificationPipeline>();
            _ = serviceCollection.AddSingleton<IPipelineHandleFactory, PipelineHandleFactory>();

            return serviceCollection;
        }

        #endregion Methods

    }

}
