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
            { "MassTransit.AspNetCoreIntegraion.Tests", null },
            { "MassTransit.QuartzIntegration.Tests", null },
            { "MassTransit.HangfireIntegration.Tests", null },
            { "MassTransit.HttpTransport.Tests", null },
            { "MassTransit.SignalR.Tests", null },
            { "MassTransit.NHibernateIntegration.Tests", null }
        };

        // Add these tests to run in Appveyor Only
        if(parameters.IsRunningOnAppVeyor)
        {
            testsToRun["MassTransit.MongoDbIntegration.Tests"] = null;
            testsToRun["MassTransit.EntityFrameworkCoreIntegration.Tests"] = "Category!=Flakey&Category!=Integration";
            testsToRun["MassTransit.EntityFrameworkIntegration.Tests"] = "Category!=Flakey&Category!=Integration";
        }

        // Add/Update these tests to run in appveyor+windows
        if(parameters.IsRunningOnAppVeyor && parameters.IsRunningOnWindows)
        {
            testsToRun["MassTransit.MartenIntegration.Tests"] = null;
            testsToRun["MassTransit.DocumentDbIntegration.Tests"] = null;
        }

        // Add/Update these tests to run in appveyor+linux
        if (parameters.IsRunningOnAppVeyor && parameters.IsRunningOnUnix)
        {
            testsToRun["MassTransit.RedisIntegration.Tests"] = null;
            testsToRun["MassTransit.RabbitMqTransport.Tests"] = null;
        }

        return new Tests{ Criteria = testsToRun };
    }
}
