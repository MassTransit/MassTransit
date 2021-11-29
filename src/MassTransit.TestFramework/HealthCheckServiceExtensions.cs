namespace MassTransit.TestFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using NUnit.Framework;
    using Serialization;


    public static class HealthCheckServiceExtensions
    {
        public static async Task WaitForHealthStatus(this HealthCheckService healthChecks, HealthStatus expectedStatus)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));

            HealthReport result;
            do
            {
                result = await healthChecks.CheckHealthAsync(cts.Token);

                await Task.Delay(1000, cts.Token);
            }
            while (result.Status != expectedStatus);

            await TestContext.Out.WriteLineAsync(result.ToJsonString());

            Assert.That(result.Status, Is.EqualTo(expectedStatus));
        }

        public static string ToJsonString(this HealthReport result)
        {
            var healthResult = new JsonObject
            {
                ["status"] = result.Status.ToString(),
                ["results"] = new JsonObject(result.Entries.Select(entry => new KeyValuePair<string, JsonNode>(entry.Key,
                    new JsonObject
                    {
                        ["status"] = entry.Value.Status.ToString(),
                        ["description"] = entry.Value.Description,
                        ["data"] = JsonSerializer.SerializeToNode(entry.Value.Data, SystemTextJsonMessageSerializer.Options)
                    })))
            };

            var options = new JsonSerializerOptions(SystemTextJsonMessageSerializer.Options)
            {
                WriteIndented = true,
            };

            return healthResult.ToJsonString(options);
        }
    }
}
