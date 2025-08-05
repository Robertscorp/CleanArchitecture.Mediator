using CleanArchitecture.Mediator;
using CleanArchitecture.Mediator.Sample.Infrastructure;
using CleanArchitecture.Mediator.Sample.Pipelines;
using CleanArchitecture.Mediator.Sample.Presenters;
using CleanArchitecture.Mediator.Sample.UseCases.CreateProduct;
using CleanArchitecture.Mediator.Sample.UseCases.GetProduct;
using CleanArchitecture.Mediator.Sample.UseCases.LegacyCreateProduct;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace CleanArchitecture.Mediator.Sample;

/// <summary>
/// This has been designed to be simple to follow.
/// </summary>
internal static class PipelineOption
{

    #region - - - - - - Methods - - - - - -

    private static async Task InvokeCreateProductUseCaseAsync(Pipeline pipeline, IServiceProvider serviceProvider)
    {
        var _PrincipalStore = (PrincipalStore)serviceProvider.GetService<IPrincipalAccessor>()!;
        _PrincipalStore.Principal = null;

        var _ServiceFactory = serviceProvider.GetRequiredService<ServiceFactory>();

        var _CreateProductPresenter = new CreateProductPresenter();
        var _CreateProductInputPort = new CreateProductInputPort
        {
            FailAuthorisation = true,
            FailInvalidCategoryBusinessRule = true,
            FailLicenceVerification = true,
            FailUniqueNameBusinessRule = true
        };

        // Create Product - Not Authenticated.
        Console.WriteLine("[Invoke Pipeline] Not Authenticated - ");
        await pipeline.InvokeAsync(_CreateProductInputPort, _CreateProductPresenter, _ServiceFactory, default);
        Console.WriteLine();
        _PrincipalStore.Principal = new ClaimsPrincipal();

        // Create Product - Not Authorised.
        Console.WriteLine("[Invoke Pipeline] Not Authorised - ");
        await pipeline.InvokeAsync(_CreateProductInputPort, _CreateProductPresenter, _ServiceFactory, default);
        Console.WriteLine();
        _CreateProductInputPort.FailAuthorisation = false;

        // Create Product - Not Licenced.
        Console.WriteLine("[Invoke Pipeline] Not Licenced - ");
        await pipeline.InvokeAsync(_CreateProductInputPort, _CreateProductPresenter, _ServiceFactory, default);
        Console.WriteLine();
        _CreateProductInputPort.FailLicenceVerification = false;

        // Create Product - Business Rule Failures.
        Console.WriteLine("[Invoke Pipeline] Business Rule Failures - ");
        await pipeline.InvokeAsync(_CreateProductInputPort, _CreateProductPresenter, _ServiceFactory, default);
        Console.WriteLine();
        _CreateProductInputPort.FailInvalidCategoryBusinessRule = false;
        _CreateProductInputPort.FailUniqueNameBusinessRule = false;

        // Create Product - Interactor Invoked.
        Console.WriteLine("[Invoke Pipeline] Valid Request - ");
        await pipeline.InvokeAsync(_CreateProductInputPort, _CreateProductPresenter, _ServiceFactory, default);
        Console.WriteLine();
    }

    public static Task InvokeDefaultPipelineAsync(IServiceProvider serviceProvider)
    {
        var _Option = default(int?);
        while (_Option.GetValueOrDefault() is < 1 or > 2)
        {
            Console.Clear();
            Console.WriteLine("We've also defined 2 Use Cases that can be invoked:");
            Console.WriteLine("\t- [1] The GetProduct Use Case, which only supports outputting a successful response.");
            Console.WriteLine("\t- [2] The CreateProduct Use Case, which supports outputting Authentication, Authorisation, and Validation failures, as well as a successful response.");
            Console.WriteLine();
            Console.Write("Please pick a use case to invoke: ");

            if (int.TryParse(Console.ReadKey().KeyChar.ToString(), out var _Val))
                _Option = _Val;
        }

        var _Pipeline = serviceProvider.GetRequiredService<DefaultPipeline>();

        Console.Clear();
        Console.WriteLine($"-- Default Pipeline --");
        Console.WriteLine();

        return _Option == 1
            ? InvokeGetProductUseCaseAsync(_Pipeline, serviceProvider)
            : InvokeCreateProductUseCaseAsync(_Pipeline, serviceProvider);
    }

    private static async Task InvokeGetProductUseCaseAsync(Pipeline pipeline, IServiceProvider serviceProvider)
    {
        var _ServiceFactory = serviceProvider.GetRequiredService<ServiceFactory>();

        var _GetProductPresenter = new GetProductPresenter();
        var _GetProductInputPort = new GetProductInputPort();

        // Get Product - Not Authenticated. Output Port doesn't support Authentication, so we expect to invoke the Interactor.
        Console.WriteLine("[Invoke Pipeline] Valid Request - ");
        await pipeline.InvokeAsync(_GetProductInputPort, _GetProductPresenter, _ServiceFactory, default);
        Console.WriteLine();
    }

    public static async Task InvokeLegacyPipelineAsync(IServiceProvider serviceProvider)
    {
        Console.Clear();
        Console.WriteLine("-- Legacy Pipeline --");
        Console.WriteLine();

        var _PrincipalStore = (PrincipalStore)serviceProvider.GetService<IPrincipalAccessor>()!;
        _PrincipalStore.Principal = null;

        var _LegacyPipeline = serviceProvider.GetService<LegacyPipeline>()!;
        var _ServiceFactory = serviceProvider.GetRequiredService<ServiceFactory>();

        var _LegacyCreateProductPresenter = new LegacyCreateProductPresenter();
        var _LegacyCreateProductInputPort = new LegacyCreateProductInputPort
        {
            FailAuthorisation = true,
            FailBusinessRuleValidation = true,
            FailInputPortValidation = true
        };

        // Create Product - Not Authenticated.
        Console.WriteLine("[Invoke Pipeline] Not Authenticated - ");
        await _LegacyPipeline.InvokeAsync(_LegacyCreateProductInputPort, _LegacyCreateProductPresenter, _ServiceFactory, default);
        Console.WriteLine();

        // Create Product - Not Authorised.
        _PrincipalStore.Principal = new ClaimsPrincipal();
        Console.WriteLine("[Invoke Pipeline] Not Authorised - ");
        await _LegacyPipeline.InvokeAsync(_LegacyCreateProductInputPort, _LegacyCreateProductPresenter, _ServiceFactory, default);
        Console.WriteLine();

        // Create Product - Input Port Validation Failure.
        _LegacyCreateProductInputPort.FailAuthorisation = false; // This should be handled through Claims.
        Console.WriteLine("[Invoke Pipeline] Invalid Input Port - ");
        await _LegacyPipeline.InvokeAsync(_LegacyCreateProductInputPort, _LegacyCreateProductPresenter, _ServiceFactory, default);
        Console.WriteLine();

        // Create Product - Business Rule Validation Failure.
        _LegacyCreateProductInputPort.FailInputPortValidation = false;
        Console.WriteLine("[Invoke Pipeline] Business Rule Violation - ");
        await _LegacyPipeline.InvokeAsync(_LegacyCreateProductInputPort, _LegacyCreateProductPresenter, _ServiceFactory, default);
        Console.WriteLine();

        // Create Product - Interactor Invoked.
        _LegacyCreateProductInputPort.FailBusinessRuleValidation = false;
        Console.WriteLine("[Invoke Pipeline] Valid Request - ");
        await _LegacyPipeline.InvokeAsync(_LegacyCreateProductInputPort, _LegacyCreateProductPresenter, _ServiceFactory, default);
        Console.WriteLine();
    }

    public static Task InvokeVerificationPipelineAsync(IServiceProvider serviceProvider)
    {
        var _Option = default(int?);
        while (_Option.GetValueOrDefault() is < 1 or > 2)
        {
            Console.Clear();
            Console.WriteLine("We've also defined 2 Use Cases that can be invoked:");
            Console.WriteLine("\t- [1] The GetProduct Use Case, which only supports outputting a successful response.");
            Console.WriteLine("\t- [2] The CreateProduct Use Case, which supports outputting Authentication, Authorisation, and Validation failures, as well as a successful response.");
            Console.WriteLine();
            Console.Write("Please pick a use case to invoke: ");

            if (int.TryParse(Console.ReadKey().KeyChar.ToString(), out var _Val))
                _Option = _Val;
        }

        var _Pipeline = serviceProvider.GetRequiredService<VerificationPipeline>();

        Console.Clear();
        Console.WriteLine($"-- Verification Pipeline --");
        Console.WriteLine();

        return _Option == 1
            ? InvokeGetProductUseCaseAsync(_Pipeline, serviceProvider)
            : InvokeCreateProductUseCaseAsync(_Pipeline, serviceProvider);
    }

    #endregion Methods

}
