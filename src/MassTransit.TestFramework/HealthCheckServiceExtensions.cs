namespace MassTransit.TestFramework
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using NUnit.Framework;


    public static class HealthCheckServiceExtensions
    {
        public static async Task WaitForHealthStatus(this HealthCheckService healthChecks, HealthStatus expectedStatus)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));

            HealthReport report;
            do
            {
                report = await healthChecks.CheckHealthAsync(cts.Token);

                await Task.Delay(1000, cts.Token);
            }
            while (report.Status != expectedStatus);

            await TestContext.Out.WriteLineAsync(report.ToJsonString());

            Assert.That(report.Status, Is.EqualTo(expectedStatus));
        }
    }
}
