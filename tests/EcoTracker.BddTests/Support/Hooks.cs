using Reqnroll;

namespace EcoTracker.BddTests.Support;

[Binding]
public sealed class Hooks
{
    [BeforeTestRun]
    public static void BeforeTestRun() =>
        Console.WriteLine($"[EcoTracker BDD] Executando contra a API {ApiHost.Mode}");

    [AfterTestRun]
    public static void AfterTestRun() => ApiHost.Shutdown();
}
