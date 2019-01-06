public class Tests
{
    public Dictionary<string,string> Criteria { get; private set; }

    public string SharedFilter { get; } = "Category!=Flakey";

    public static Tests GetTests(BuildParameters parameters)
    {
        // Ran in all environments
        var testsToRun = new Dictionary<string, string>
        {
            { "MassTransit.Tests", null },
            { "MassTransit.Containers.Tests", null },
            { "MassTransit.ApplicationInsights.Tests", null },
            { "MassTransit.AutomatonymousIntegration.Tests", "Category!=DocumentDb&Category!=EntityFramework" }, // DocumentDb is only installed on appveyor windows vm, or develoer if they put DocDb emulator
            { "MassTransit.QuartzIntegration.Tests", null },
            { "MassTransit.HttpTransport.Tests", null },
            { "MassTransit.SignalR.Tests", null },
            { "MassTransit.NHibernateIntegration.Tests", null },
            // { "MassTransit.AzureServiceBusTransport.Tests", "Category!=Flakey" }
        };

        // Add these tests to run in Appveyor Only
        if(parameters.IsRunningOnAppVeyor)
        {
            testsToRun["MassTransit.MongoDbIntegration.Tests"] = null;
            testsToRun["MassTransit.EntityFrameworkCoreIntegration.Tests"] = null;
            testsToRun["MassTransit.EntityFrameworkIntegration.Tests"] = null;
        }

        // Add/Update these tests to run in appveyor+windows
        if(parameters.IsRunningOnAppVeyor && parameters.IsRunningOnWindows)
        {
            testsToRun["MassTransit.MartenIntegration.Tests"] = null;
            testsToRun["MassTransit.DocumentDbIntegration.Tests"] = null;
            testsToRun["MassTransit.AutomatonymousIntegration.Tests"] = null; // We can include the DocumentDb and EF tests
        }

        // Add/Update these tests to run in appveyor+linux
        if (parameters.IsRunningOnAppVeyor && parameters.IsRunningOnUnix)
        {
            testsToRun["MassTransit.RedisIntegration.Tests"] = null;
            testsToRun["MassTransit.RabbitMqTransport.Tests"] = null;
            testsToRun["MassTransit.AutomatonymousIntegration.Tests"] = "Category!=DocumentDb";
        }

        return new Tests{ Criteria = testsToRun };
    }
}
