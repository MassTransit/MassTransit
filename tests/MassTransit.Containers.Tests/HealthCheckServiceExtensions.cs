namespace MassTransit.Containers.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;


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

            await TestContext.Out.WriteLineAsync(FormatHealthCheck(result));

            Assert.That(result.Status, Is.EqualTo(expectedStatus));
        }

        static string FormatHealthCheck(HealthReport result)
        {
            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("results", new JObject(result.Entries.Select(entry => new JProperty(entry.Key, new JObject(
                    new JProperty("status", entry.Value.Status.ToString()),
                    new JProperty("description", entry.Value.Description),
                    new JProperty("data", JObject.FromObject(entry.Value.Data))))))));

            return json.ToString(Formatting.Indented);
        }
    }
}
