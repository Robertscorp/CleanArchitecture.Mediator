using CleanArchitecture.Mediator.Sample;
using CleanArchitecture.Mediator.Sample.Pipelines;
using Microsoft.Extensions.DependencyInjection;

using var _ServiceProvider
    = new ServiceCollection()
            .AddCleanArchitectureMediator()
            .AddSingleton<VerificationPipelineInvoker>()
            .BuildServiceProvider();

var _Option = default(int?);
while (_Option.GetValueOrDefault() is < 1 or > 2)
{
    Console.Clear();
    Console.WriteLine("Welcome to the CLAM (CleanArchitecture.Mediator) sample project.");
    Console.WriteLine("In this sample, we've defined 2 pipelines that can be invoked:");
    Console.WriteLine("\t[1] The default pipeline, consisting of Authentication, Authorisation Policy Validation, Licence Policy Validation, Input Port Validation, Business Rule Evaluation, and Interactor Invocation.");
    Console.WriteLine("\t[2] The verification pipeline, consisting of Authentication, Authorisation Policy Validation, Licence Policy Validation, Input Port Validation, Business Rule Evaluation, and Verification Success.");
    Console.WriteLine();
    Console.Write("Please pick a pipeline to invoke: ");

    if (int.TryParse(Console.ReadKey().KeyChar.ToString(), out var _Val))
        _Option = _Val;
}

using var _Scope = _ServiceProvider.CreateScope();
var _ScopedProvider = _Scope.ServiceProvider;

switch (_Option)
{
    case 1: await PipelineOption.InvokeDefaultPipelineAsync(_ScopedProvider); break;
    case 2: await PipelineOption.InvokeVerificationPipelineAsync(_ScopedProvider); break;
}

Console.WriteLine("Press any key to exit.");
Console.ReadKey();
