// See https://aka.ms/new-console-template for more information

using CleanArchitecture.Sample.Infrastructure;
using CleanArchitecture.Sample.Pipeline;
using CleanArchitecture.Sample.Presenters;
using CleanArchitecture.Sample.UseCases.CreateProduct;
using CleanArchitecture.Sample.UseCases.GetProduct;
using CleanArchitecture.Services.Infrastructure;
using CleanArchitecture.Services.Pipeline;
using CleanArchitecture.Services.Pipeline.Authentication;
using CleanArchitecture.Services.Pipeline.Authorisation;
using CleanArchitecture.Services.Pipeline.Infrastructure;
using CleanArchitecture.Services.Pipeline.Validation;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

var _ClaimsPrincipalProvider = new AuthenticatedClaimsPrincipalProvider(); // Currently Unauthenticated.

using var _ServiceProvider = new ServiceCollection()
                                    // Register the DI Service
                                    .AddScoped<UseCaseServiceResolver>(serviceProvider => serviceProvider.GetService)

                                    // Register the Pipeline
                                    .AddScoped<IUseCaseInvoker, UseCaseInvoker>()
                                    .AddScoped<IUseCaseElement, AuthenticationUseCaseElement>()
                                    .AddScoped<IUseCaseElement, AuthorisationUseCaseElement<AuthorisationResult>>()
                                    .AddScoped<IUseCaseElement, InputPortValidatorUseCaseElement<ValidationResult>>()
                                    .AddScoped<IUseCaseElement, BusinessRuleValidatorUseCaseElement<ValidationResult>>()
                                    .AddScoped<IUseCaseElement, InteractorUseCaseElement>()

                                    // Register the Infrastructure
                                    .AddSingleton<IAuthenticatedClaimsPrincipalProvider>(_ClaimsPrincipalProvider)

                                    // Register the Use Cases
                                    .AddScoped<IUseCaseAuthorisationEnforcer<CreateProductInputPort, AuthorisationResult>, CreateProductAuthorisationEnforcer>()
                                    .AddScoped<IUseCaseInputPortValidator<CreateProductInputPort, ValidationResult>, CreateProductInputPortValidator>()
                                    .AddScoped<IUseCaseBusinessRuleValidator<CreateProductInputPort, ValidationResult>, CreateProductBusinessRuleValidator>()
                                    .AddScoped<IUseCaseInteractor<CreateProductInputPort, ICreateProductOutputPort>, CreateProductInteractor>()
                                    .AddScoped<IUseCaseInteractor<GetProductInputPort, IGetProductOutputPort>, GetProductInteractor>()
                                    .BuildServiceProvider();

using var _Scope = _ServiceProvider.CreateScope();
var _ScopedProvider = _Scope.ServiceProvider;

var _UseCaseInvoker = _ServiceProvider.GetService<IUseCaseInvoker>()!;

// Get Product - Not Authenticated. Output Port doesn't support Authentication, so we expect to invoke the Interactor.
await _UseCaseInvoker.InvokeUseCaseAsync(new GetProductInputPort(), new GetProductPresenter(), default);

// Create Product - Not Authenticated.
await _UseCaseInvoker.InvokeUseCaseAsync(new CreateProductInputPort(), new CreateProductPresenter(), default);

// We're now Authenticated.
_ClaimsPrincipalProvider.AuthenticatedClaimsPrincipal = new ClaimsPrincipal();

// Create Product - Not Authorised.
await _UseCaseInvoker.InvokeUseCaseAsync(new CreateProductInputPort { FailAuthorisation = true }, new CreateProductPresenter(), default);

// Create Product - Input Port Validation Failure.
await _UseCaseInvoker.InvokeUseCaseAsync(new CreateProductInputPort { FailInputPortValidation = true }, new CreateProductPresenter(), default);

// Create Product - Business Rule Validation Failure.
await _UseCaseInvoker.InvokeUseCaseAsync(new CreateProductInputPort { FailBusinessRuleValidation = true }, new CreateProductPresenter(), default);

// Create Product - Interactor Invoked.
await _UseCaseInvoker.InvokeUseCaseAsync(new CreateProductInputPort(), new CreateProductPresenter(), default);

Console.WriteLine("Press 'enter' to finish.");
_ = Console.ReadLine();
