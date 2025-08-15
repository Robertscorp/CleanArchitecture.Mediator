using CleanArchitecture.Mediator;
using CleanArchitecture.Mediator.Sample.Application.Services.Pipelines;
using CleanArchitecture.Mediator.Sample.Application.UseCases.SampleCreate;
using CleanArchitecture.Mediator.Sample.Application.UseCases.SampleGet;
using CleanArchitecture.Mediator.Sample.Framework.Infrastructure;
using CleanArchitecture.Mediator.Sample.InterfaceAdapters.Presenters;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace CleanArchitecture.Mediator.Sample;

/// <summary>
/// This has been designed to be simple to follow.
/// </summary>
internal static class PipelineOption
{

    #region - - - - - - Methods - - - - - -

    private static async Task InvokeSampleCreateUseCaseAsync(Pipeline pipeline, IServiceProvider serviceProvider)
    {
        var _PrincipalStore = (PrincipalStore)serviceProvider.GetService<IPrincipalAccessor>()!;
        _PrincipalStore.Principal = null;

        var _ServiceFactory = serviceProvider.GetRequiredService<ServiceFactory>();

        var _SampleCreatePresenter = new SampleCreatePresenter()
        {
            WarnOnInputPortValidationFailure = true
        };

        var _SampleCreateInputPort = new SampleCreateInputPort
        {
            FailAuthorisation = true,
            FailInvalidCategoryBusinessRule = true,
            FailInputPortValidation = true,
            FailLicenceVerification = true,
            FailUniqueNameBusinessRule = true
        };

        // Sample Create - Not Authenticated.
        Console.WriteLine("[Invoke Pipeline] Not Authenticated - ");
        await pipeline.InvokeAsync(_SampleCreateInputPort, _SampleCreatePresenter, _ServiceFactory, default);
        Console.WriteLine();
        _PrincipalStore.Principal = new ClaimsPrincipal();

        // Sample Create - Not Authorised.
        Console.WriteLine("[Invoke Pipeline] Not Authorised - ");
        await pipeline.InvokeAsync(_SampleCreateInputPort, _SampleCreatePresenter, _ServiceFactory, default);
        Console.WriteLine();
        _SampleCreateInputPort.FailAuthorisation = false;

        // Sample Create - Not Licenced.
        Console.WriteLine("[Invoke Pipeline] Not Licenced - ");
        await pipeline.InvokeAsync(_SampleCreateInputPort, _SampleCreatePresenter, _ServiceFactory, default);
        Console.WriteLine();
        _SampleCreateInputPort.FailLicenceVerification = false;

        // Sample Create - Invalid Input Port.
        Console.WriteLine("[Invoke Pipeline] Invalid Input Port - [Warn only]");
        await pipeline.InvokeAsync(_SampleCreateInputPort, _SampleCreatePresenter, _ServiceFactory, default);
        Console.WriteLine();
        _SampleCreatePresenter.WarnOnInputPortValidationFailure = false;

        // Sample Create - Invalid Input Port.
        Console.WriteLine("[Invoke Pipeline] Invalid Input Port - [Fail]");
        await pipeline.InvokeAsync(_SampleCreateInputPort, _SampleCreatePresenter, _ServiceFactory, default);
        Console.WriteLine();
        _SampleCreateInputPort.FailInputPortValidation = false;

        // Sample Create - Business Rule Failures.
        Console.WriteLine("[Invoke Pipeline] Business Rule Failures - ");
        await pipeline.InvokeAsync(_SampleCreateInputPort, _SampleCreatePresenter, _ServiceFactory, default);
        Console.WriteLine();
        _SampleCreateInputPort.FailInvalidCategoryBusinessRule = false;
        _SampleCreateInputPort.FailUniqueNameBusinessRule = false;

        // Sample Create - Interactor Invoked.
        Console.WriteLine("[Invoke Pipeline] Valid Request - ");
        await pipeline.InvokeAsync(_SampleCreateInputPort, _SampleCreatePresenter, _ServiceFactory, default);
        Console.WriteLine();
    }

    public static Task InvokeDefaultPipelineAsync(IServiceProvider serviceProvider)
    {
        var _Option = default(int?);
        while (_Option.GetValueOrDefault() is < 1 or > 2)
        {
            Console.Clear();
            Console.WriteLine("We've also defined 2 Use Cases that can be invoked:");
            Console.WriteLine("\t- [1] The SampleGet Use Case, which only supports outputting a successful response.");
            Console.WriteLine("\t- [2] The SampleCreate Use Case, which supports outputting all failures, as well as a successful response.");
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
            ? InvokeSampleGetUseCaseAsync(_Pipeline, serviceProvider)
            : InvokeSampleCreateUseCaseAsync(_Pipeline, serviceProvider);
    }

    private static async Task InvokeSampleGetUseCaseAsync(Pipeline pipeline, IServiceProvider serviceProvider)
    {
        var _ServiceFactory = serviceProvider.GetRequiredService<ServiceFactory>();

        var _SampleGetPresenter = new SampleGetPresenter();
        var _SampleGetInputPort = new SampleGetInputPort();

        // Sample Get - Not Authenticated. Output Port doesn't support Authentication, so we expect to invoke the Interactor.
        Console.WriteLine("[Invoke Pipeline] Valid Request - ");
        await pipeline.InvokeAsync(_SampleGetInputPort, _SampleGetPresenter, _ServiceFactory, default);
        Console.WriteLine();
    }

    public static Task InvokeVerificationPipelineAsync(IServiceProvider serviceProvider)
    {
        var _Option = default(int?);
        while (_Option.GetValueOrDefault() is < 1 or > 2)
        {
            Console.Clear();
            Console.WriteLine("We've also defined 2 Use Cases that can be invoked:");
            Console.WriteLine("\t- [1] The SampleGet Use Case, which only supports outputting a successful response.");
            Console.WriteLine("\t- [2] The SampleCreate Use Case, which supports outputting Authentication, Authorisation, and Validation failures, as well as a successful response.");
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
            ? InvokeSampleGetUseCaseAsync(_Pipeline, serviceProvider)
            : InvokeSampleCreateUseCaseAsync(_Pipeline, serviceProvider);
    }

    #endregion Methods

}
