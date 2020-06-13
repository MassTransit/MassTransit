public class Tests
{
    public Dictionary<string,string> Criteria { get; private set; }

    public string SharedFilter { get; } = "Category!=Flakey";

    public static Tests GetTests(BuildParameters parameters)
    {
        // Ran in all environments, because they work with out any additional dependencies
        var testsToRun = new Dictionary<string, string>
        {
            { "MassTransit.Tests", null },
            { "MassTransit.Containers.Tests", null },
            { "MassTransit.Analyzers.Tests", null },
            { "MassTransit.QuartzIntegration.Tests", null },
            { "MassTransit.HangfireIntegration.Tests", null },
            { "MassTransit.SignalR.Tests", null },
            { "MassTransit.NHibernateIntegration.Tests", null }
        };

        // Add these tests to run in Appveyor Only
        if(parameters.IsRunningOnAppVeyor)
        {
            if(parameters.IsRunningOnUnix)
            {
                testsToRun["MassTransit.RedisIntegration.Tests"] = null;
                testsToRun["MassTransit.RabbitMqTransport.Tests"] = null;
            }

            if(parameters.IsRunningOnWindows)
            {
                testsToRun.Clear();
                testsToRun["MassTransit.MongoDbIntegration.Tests"] = null;
                testsToRun["MassTransit.EntityFrameworkCoreIntegration.Tests"] = "Category!=Flakey&Category!=Integration";
                testsToRun["MassTransit.EntityFrameworkIntegration.Tests"] = "Category!=Flakey&Category!=Integration";
                testsToRun["MassTransit.MartenIntegration.Tests"] = null;
                testsToRun["MassTransit.Azure.Cosmos.Tests"] = null;
            }
        }

        return new Tests{ Criteria = testsToRun };
    }
}
