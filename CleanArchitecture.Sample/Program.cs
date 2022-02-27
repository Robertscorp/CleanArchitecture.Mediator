using CleanArchitecture.Mediator;
using CleanArchitecture.Mediator.Authentication;
using CleanArchitecture.Mediator.Infrastructure;
using CleanArchitecture.Sample;
using CleanArchitecture.Sample.Infrastructure;
using CleanArchitecture.Sample.Presenters;
using CleanArchitecture.Sample.UseCases.CreateProduct;
using CleanArchitecture.Sample.UseCases.GetProduct;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

using var _ServiceProvider
    = new ServiceCollection()
            .AddScoped<UseCaseServiceResolver>(serviceProvider => serviceProvider.GetService)
            .AddCleanArchitectureServices()
            .BuildServiceProvider();

using var _Scope = _ServiceProvider.CreateScope();
var _ScopedProvider = _Scope.ServiceProvider;

// Currently Unauthenticated.
var _ClaimsPrincipalProvider = (AuthenticatedClaimsPrincipalProvider)_ScopedProvider.GetService<IAuthenticatedClaimsPrincipalProvider>()!;

var _UseCaseInvoker = _ScopedProvider.GetService<IUseCaseInvoker>()!;

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
