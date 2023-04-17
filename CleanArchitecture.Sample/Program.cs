using CleanArchitecture.Mediator;
using CleanArchitecture.Sample;
using CleanArchitecture.Sample.Infrastructure;
using CleanArchitecture.Sample.Pipelines;
using CleanArchitecture.Sample.Presenters;
using CleanArchitecture.Sample.UseCases.CreateProduct;
using CleanArchitecture.Sample.UseCases.GetProduct;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

using var _ServiceProvider
    = new ServiceCollection()
            .AddCleanArchitectureMediator()
            .BuildServiceProvider();

using var _Scope = _ServiceProvider.CreateScope();
var _ScopedProvider = _Scope.ServiceProvider;

// Currently Unauthenticated.
var _ClaimsPrincipalProvider = (AuthenticatedClaimsPrincipalProvider)_ScopedProvider.GetService<IAuthenticatedClaimsPrincipalProvider>()!;

var _DefaultPipeline = _ScopedProvider.GetService<DefaultPipeline>()!;
var _VerificationPipeline = _ScopedProvider.GetService<VerificationPipeline>()!;

Console.WriteLine("Welcome to the CLAM (CleanArchitecture.Mediator) sample project.");
Console.WriteLine("In this sample, we've defined 2 pipelines that can be invoked:");
Console.WriteLine("    - The default pipeline, consisting of Authentication, Authorisation, Validation, and Interactor Invocation.");
Console.WriteLine("    - The verification pipeline, consisting of Authentication, Authorisation, and Validation.");
Console.WriteLine();
Console.WriteLine("We've also defined 2 Use Cases that can be invoked:");
Console.WriteLine("    - The GetProduct Use Case, which only supports outputting a successful response.");
Console.WriteLine("    - The CreateProduct Use Case, which supports outputting Authentication, Authorisation, and Validation failures, as well as a successful response.");
Console.WriteLine();
Console.WriteLine("We're currently Unauthenticated.");
Console.WriteLine("This means that if we invoke the CreateProduct Use Case, we should expect to get an Authentication failure.");
Console.WriteLine("However, since GetProduct doesn't support Authentication, we can't get an Authentication failure.");
Console.WriteLine();
Console.WriteLine("Now we're up to invoking the Pipelines. ");
Console.WriteLine("Since the verification pipeline doesn't have an Interactor Invocation Pipe, it won't invoke the Interactor.");
Console.WriteLine();
Console.WriteLine("[GET PRODUCT]");
Console.WriteLine();

// Get Product - Not Authenticated. Output Port doesn't support Authentication, so we expect to invoke the Interactor.
await _DefaultPipeline.InvokeAsync(new GetProductInputPort(), new GetProductPresenter(), default);

// Get Product - Not Authenticated. Output Port doesn't support Authentication, so we don't expect to invoke anything.
await _VerificationPipeline.InvokeAsync(new GetProductInputPort(), new GetProductPresenter(), default);

Console.WriteLine();
Console.WriteLine("[CREATE PRODUCT]");
Console.WriteLine();

var _CreateProductPresenter = new CreateProductPresenter();
var _CreateProductInputPort = new CreateProductInputPort
{
    FailAuthorisation = true,
    FailValidation = true
};

// Create Product - Not Authenticated.
await _DefaultPipeline.InvokeAsync(_CreateProductInputPort, _CreateProductPresenter, default);

// Create Product - Not Authenticated.
await _VerificationPipeline.InvokeAsync(_CreateProductInputPort, _CreateProductPresenter, default);

Console.WriteLine();
Console.Write("Let's Authenticate so we stop getting Authentication failures...");

// We're now Authenticated.
_ClaimsPrincipalProvider.AuthenticatedClaimsPrincipal = new ClaimsPrincipal();

Console.WriteLine(" Done!");
Console.WriteLine();

// Create Product - Not Authorised.
await _DefaultPipeline.InvokeAsync(_CreateProductInputPort, _CreateProductPresenter, default);

// Create Product - Not Authorised.
await _VerificationPipeline.InvokeAsync(_CreateProductInputPort, _CreateProductPresenter, default);

Console.WriteLine();
Console.Write("Let's stop getting Authorisation failures...");
_CreateProductInputPort.FailAuthorisation = false; // This should be handled through Claims.
Console.WriteLine(" Done!");
Console.WriteLine();

// Create Product - Validation Failure.
await _DefaultPipeline.InvokeAsync(_CreateProductInputPort, _CreateProductPresenter, default);

// Create Product - Validation Failure.
await _VerificationPipeline.InvokeAsync(_CreateProductInputPort, _CreateProductPresenter, default);

Console.WriteLine();
Console.Write("Let's make our Input Port valid...");
_CreateProductInputPort.FailValidation = false;
Console.WriteLine(" Done!");
Console.WriteLine("As a reminder, the verification pipeline doesn't have an Interactor Invocation Pipe so it won't invoke the Interactor.");
Console.WriteLine();

// Create Product - Interactor Invoked.
await _DefaultPipeline.InvokeAsync(_CreateProductInputPort, _CreateProductPresenter, default);

// Create Product - No services Invoked.
await _VerificationPipeline.InvokeAsync(_CreateProductInputPort, _CreateProductPresenter, default);

Console.WriteLine("Press 'enter' to finish.");
_ = Console.ReadLine();
